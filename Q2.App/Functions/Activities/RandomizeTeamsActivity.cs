using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;

namespace Q2.App.Functions.Activities;

public record RandomizeTeamsActivityInput(IList<PlayerWithElo> Players);

public record RandomizeTeamsActivityOutput(Team Team1, Team Team2);

public record PlayerWithElo(string Id, int Elo);

public record Team(IList<string> PlayerIds, int AverageElo);

public static class RandomizeTeamsActivity
{
    [Function(nameof(RandomizeTeamsActivity))]
    public static async Task<RandomizeTeamsActivityOutput> RunAsync(
        [ActivityTrigger] RandomizeTeamsActivityInput input)
    {
        var random = new Random();
        var sorted = input.Players.OrderBy(_ => random.Next()).ToList();

        var halfIndex = sorted.Count / 2;
        var team1Players = sorted.Take(halfIndex).ToList();
        var team2Players = sorted.Skip(halfIndex).ToList();

        var team1Elo = team1Players.Sum(x => x.Elo) / team1Players.Count;
        var team2Elo = team2Players.Sum(x => x.Elo) / team2Players.Count;

        var team1 = new Team([.. team1Players.Select(x => x.Id)], team1Elo);
        var team2 = new Team([.. team2Players.Select(x => x.Id)], team2Elo);

        return new RandomizeTeamsActivityOutput(team1, team2);
    }
}

public static class RandomizeTeamsActivityExtensions
{
    public static async Task<RandomizeTeamsActivityOutput> RandomizeTeamsAsync(
        this TaskOrchestrationContext ctx,
        IList<PlayerWithElo> players)
    {
        return await ctx.CallActivityAsync<RandomizeTeamsActivityOutput>(
            nameof(RandomizeTeamsActivity),
            new RandomizeTeamsActivityInput(players));
    }
}
