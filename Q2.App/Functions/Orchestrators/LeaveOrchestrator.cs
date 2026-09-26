using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Microsoft.DurableTask.Entities;
using Q2.App.Common;
using Q2.App.Functions.Activities;
using Q2.App.Functions.Entities;

namespace Q2.App.Functions.Orchestrators;

public record LeaveOrchestratorInput(string ApplicationId, string InteractionToken, string ChannelId, string PlayerId);

public static class LeaveOrchestrator
{
    [Function(nameof(LeaveOrchestrator))]
    public static async Task<bool> RunAsync(
        [OrchestrationTrigger] TaskOrchestrationContext ctx,
        LeaveOrchestratorInput input)
    {
        MessageInteractionCallback callback;
        try
        {
            var state = await ctx.QueueDequeueAsync(input.ChannelId, input.PlayerId);
            callback = SuccessResponse(input.PlayerId, state.Players.Count, state.Size);
        }
        catch (EntityOperationFailedException ex) when (ex.FailureDetails.IsCausedBy<InvalidOperationException>())
        {
            callback = ErrorResponse(ex.FailureDetails.ErrorMessage);
        }

        try
        {
            await ctx.PlayerLeaveQueueAsync(input.PlayerId);
        }
        catch (Exception)
        {
            // Ignore any exception here as no exception can leave the entity in an invalid state for having left the queue
        }

        return await ctx.EditOriginalInteractionResponseAsync(input.ApplicationId, input.InteractionToken, callback);
    }

    private static MessageInteractionCallback SuccessResponse(string userId, int count, int size) => new(
        MessageFlags.IsComponentsV2,
        [
            new ContainerComponent([
                new TextDisplayComponent($"<@{userId}> has left the queue! There are currently **{count}/{size}** in the queue."),
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

public static class LeaveOrchestratorExtensions
{
    public static async Task ScheduleNewLeaveOrchestratorInstanceAsync(
        this DurableTaskClient ctx,
        string applicationId,
        string interactionToken,
        string channelId,
        string playerId)
    {
        var input = new LeaveOrchestratorInput(applicationId, interactionToken, channelId, playerId);
        await ctx.ScheduleNewOrchestrationInstanceAsync(nameof(LeaveOrchestrator), input);
    }
}
