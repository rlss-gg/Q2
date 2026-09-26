using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Q2.App.Common;
using Q2.App.Functions.Activities;
using Q2.App.Functions.Entities;

namespace Q2.App.Functions.Orchestrators;

public record StatusOrchestratorInput(string ApplicationId, string InteractionToken, string ChannelId);

public static class StatusOrchestrator
{
    [Function(nameof(StatusOrchestrator))]
    public static async Task<bool> RunAsync(
        [OrchestrationTrigger] TaskOrchestrationContext ctx,
        StatusOrchestratorInput input)
    {
        var res = await ctx.QueueGetStateAsync(input.ChannelId);
        var callback = res switch
        {
            QueueState state => SuccessResponse(state.Players.Count, state.Size),
            null => ErrorResponse("There is no queue in this channel."),
        };

        return await ctx.EditOriginalInteractionResponseAsync(input.ApplicationId, input.InteractionToken, callback);
    }

    private static MessageInteractionCallback SuccessResponse(int count, int size) => new(
        MessageFlags.IsComponentsV2,
        [
            new ContainerComponent([
                new TextDisplayComponent($"There are currently **{count}/{size}** in the queue."),
            ]),
        ]);

    private static MessageInteractionCallback ErrorResponse(string message) => new(
        MessageFlags.IsComponentsV2 | MessageFlags.Ephemeral,
        [
            new ContainerComponent([
                new TextDisplayComponent($"Error: {message}"),
            ]),
        ]);
}

public static class StatusOrchestratorExtensions
{
    public static async Task ScheduleNewStatusOrchestratorInstanceAsync(
        this DurableTaskClient ctx,
        string applicationId,
        string interactionToken,
        string channelId,
        string playerId)
    {
        var input = new StatusOrchestratorInput(applicationId, interactionToken, channelId);
        await ctx.ScheduleNewOrchestrationInstanceAsync(nameof(StatusOrchestrator), input);
    }
}
