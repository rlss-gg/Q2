using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Q2.App.Functions.Orchestrators;

namespace Q2.App.Functions.Activities;

public record CreateMatchActivityInput(
    string matchId,
    string ApplicationId,
    string InteractionToken,
    string ChannelId,
    HashSet<string> PlayerIds);

public static class CreateMatchActivity
{
    [Function(nameof(CreateMatchActivity))]
    public static async Task RunAsync(
        [ActivityTrigger] CreateMatchActivityInput input,
        [DurableClient] DurableTaskClient durableClient)
    {
        await durableClient.ScheduleNewMatchOrchestratorInstanceAsync(
            input.matchId,
            input.ApplicationId,
            input.InteractionToken,
            input.ChannelId,
            input.PlayerIds);
    }
}

public static class CreateMatchActivityExtensions
{
    public static async Task CreateMatchAsync(
        this TaskOrchestrationContext ctx,
        string matchId,
        string applicationId,
        string interactionToken,
        string channelId,
        HashSet<string> playerIds)
    {
        await ctx.CallActivityAsync(
            nameof(CreateMatchActivity),
            new CreateMatchActivityInput(matchId, applicationId, interactionToken, channelId, playerIds));
    }
}
