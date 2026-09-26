using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Q2.App.Callbacks;
using Q2.App.Common;
using Q2.App.Functions.Activities;
using Q2.App.Functions.Entities;
using System.Diagnostics.CodeAnalysis;

namespace Q2.App.Functions.Orchestrators;

public record MatchOrchestratorInput(
    string ApplicationId,
    string InteractionToken,
    string ChannelId,
    HashSet<string> PlayerIds);

public record MatchOrchestratorReadyEvent(
    string InteractionToken,
    string PlayerId);

public record MatchOrchestratorScoreReportEvent(
    string InteractionToken,
    string PlayerId,
    int PlayerScore,
    int OpponentScore);

public static class MatchOrchestrator
{
    public const int TimeoutMinutes = 5;

    public const string InstanceIdPrefix = "match-";

    public static string InstanceId(string matchId) => $"{InstanceIdPrefix}{matchId}";

    public static string MatchId(string instanceId) => instanceId[InstanceIdPrefix.Length..];

    [Function(nameof(MatchOrchestrator))]
    public static async Task<bool> RunAsync(
        [OrchestrationTrigger] TaskOrchestrationContext ctx,
        MatchOrchestratorInput input)
    {
        var matchId = MatchId(ctx.InstanceId);
        var playersReady = input.PlayerIds.ToDictionary(id => id, _ => false);

        // Update users to be in match
        await Task.WhenAll(input.PlayerIds.Select(id => ctx.PlayerSetMatchAsync(id, matchId)));

        // Wait for players to ready up
        var timeout = ctx.CurrentUtcDateTime.AddMinutes(TimeoutMinutes);

        var queueFilledCallback = new QueueFilledCallback(matchId, playersReady, timeout, TimeoutMinutes);
        await ctx.CreateFollowUpMessageAsync(input.ApplicationId, input.InteractionToken, queueFilledCallback.ToCallback());

        using var timeoutCts = new CancellationTokenSource();
        var timeoutTimer = ctx.CreateTimer(timeout, timeoutCts.Token);

        while (true)
        {
            var readyEvent = ctx.WaitForExternalEvent<MatchOrchestratorReadyEvent>(nameof(MatchOrchestratorReadyEvent));
            var winner = await Task.WhenAny(readyEvent, timeoutTimer);

            if (winner == readyEvent)
            {
                if (playersReady.TryGetValue(readyEvent.Result.PlayerId, out var ready) && !ready)
                {
                    playersReady[readyEvent.Result.PlayerId] = true;

                    await ctx.EditOriginalInteractionResponseAsync(
                        input.ApplicationId,
                        readyEvent.Result.InteractionToken,
                        queueFilledCallback.ToCallback());
                }

                // TODO: Consider error message if someone already ready or not in match presses ready button

                if (playersReady.All(kvp => kvp.Value))
                {
                    timeoutCts.Cancel();
                    break;
                }
            }
            else if (winner == timeoutTimer)
            {
                break;
            }
        }

        // Cancel match if not all players ready
        if (!playersReady.All(kvp => kvp.Value))
        {
            await ctx.CreateFollowUpMessageAsync(
                input.ApplicationId,
                input.InteractionToken,
                new QueueTimedOutCallback([.. input.PlayerIds]).ToCallback());

            return false;
        }

        // Create random teams and start match
        var nullablePlayerStates = await Task.WhenAll(input.PlayerIds.Select(ctx.PlayerGetStateAsync));

        if (nullablePlayerStates.Any(s => s is null))
        {
            await ctx.CreateFollowUpMessageAsync(
                input.ApplicationId,
                input.InteractionToken,
                new MatchCancelledMissingPlayerCallback([.. input.PlayerIds]).ToCallback());

            return false;
        }

        var playersWithElo = nullablePlayerStates
            .Cast<PlayerState>()
            .Select(s => new PlayerWithElo(s.Id, s.Elo))
            .ToList();

        var teams = await ctx.RandomizeTeamsAsync(playersWithElo);

        await ctx.CreateFollowUpMessageAsync(
            input.ApplicationId,
            input.InteractionToken,
            new MatchStartedCallback(teams.Team1, teams.Team2).ToCallback());

        // Wait for score report then update player elos
        var report = await ctx.WaitForExternalEvent<MatchOrchestratorScoreReportEvent>(nameof(MatchOrchestratorScoreReportEvent));
        var reporterTeam = teams.Team1.PlayerIds.Contains(report.PlayerId) ? teams.Team1 : teams.Team2;
        var team1Score = reporterTeam == teams.Team1 ? report.PlayerScore : report.OpponentScore;
        var team2Score = reporterTeam == teams.Team1 ? report.OpponentScore : report.PlayerScore;

        var winningTeam = team1Score > team2Score ? teams.Team1 : teams.Team2;
        var eloChange = Elo.CalculateChange(teams.Team1.AverageElo, teams.Team2.AverageElo, winningTeam == teams.Team1);

        var updatedPlayerElos = playersWithElo
            .Select(p =>
            {
                var isTeamA = teams.Team1.PlayerIds.Any(id => id == p.Id);
                var change = (int)(isTeamA ? eloChange.A : eloChange.B);
                var elo = p.Elo + change;
                return new PlayerMatchResult(p.Id, elo, change);
            })
            .ToList();

        await Task.WhenAll(updatedPlayerElos.Select(p => ctx.PlayerSetEloAsync(p.Id, p.Elo)));

        // Mark players as finished with match
        await Task.WhenAll(input.PlayerIds.Select(id => ctx.PlayerSetMatchAsync(id, null)));

        // Send match completed message
        await ctx.EditOriginalInteractionResponseAsync(
            input.ApplicationId,
            report.InteractionToken,
            new MatchCompletedCallback(updatedPlayerElos, team1Score, team2Score).ToCallback());

        return true;
    }
}

public static class MatchOrchestratorExtensions
{
    public static async Task ScheduleNewMatchOrchestratorInstanceAsync(
        this DurableTaskClient ctx,
        string matchId,
        string applicationId,
        string interactionToken,
        string channelId,
        HashSet<string> playerIds)
    {
        var input = new MatchOrchestratorInput(applicationId, interactionToken, channelId, playerIds);

        await ctx.ScheduleNewOrchestrationInstanceAsync(
            nameof(MatchOrchestrator),
            input,
            new StartOrchestrationOptions(InstanceId: MatchOrchestrator.InstanceId(matchId)));
    }
}
