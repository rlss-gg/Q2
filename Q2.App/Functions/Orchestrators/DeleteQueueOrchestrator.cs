using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Microsoft.DurableTask.Entities;
using Q2.App.Common;
using Q2.App.Functions.Activities;
using Q2.App.Functions.Entities;

namespace Q2.App.Functions.Orchestrators;

public record DeleteQueueOrchestratorInput(string ApplicationId, string InteractionToken, string ChannelId);

public static class DeleteQueueOrchestrator
{
    [Function(nameof(DeleteQueueOrchestrator))]
    public static async Task<bool> RunAsync(
        [OrchestrationTrigger] TaskOrchestrationContext ctx,
        DeleteQueueOrchestratorInput input)
    {
        // Ensure queue exists
        var state = await ctx.QueueGetStateAsync(input.ChannelId);

        if (state is null)
        {
            await ctx.EditOriginalInteractionResponseAsync(
                input.ApplicationId,
                input.InteractionToken,
                ErrorResponse("Queue not found."));

            return false;
        }

        // Lock queue and players (if any) to prevent race conditions then delete
        var locks = new List<EntityInstanceId> { QueueEntity.Id(input.ChannelId) };

        foreach (var playerId in state.Players)
            locks.Add(PlayerEntity.Id(playerId));

        await using (await ctx.Entities.LockEntitiesAsync(locks))
        {
            await ctx.QueueDeleteAsync(input.ChannelId);

            foreach (var playerId in state.Players)
            {
                try
                {
                    await ctx.PlayerLeaveQueueAsync(playerId);
                }
                catch
                {
                    // Ignore errors as none are critical to the queue deletion
                }
            }
        }

        await ctx.EditOriginalInteractionResponseAsync(
            input.ApplicationId,
            input.InteractionToken,
            SuccessResponse());

        return true;
    }

    private static MessageInteractionCallback SuccessResponse() => new(
        MessageFlags.IsComponentsV2 | MessageFlags.Ephemeral,
        [
            new ContainerComponent([
                new TextDisplayComponent($"Deleted queue."),
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

public static class DeleteQueueOrchestratorExtensions
{
    public static async Task ScheduleNewDeleteQueueOrchestratorInstanceAsync(
        this DurableTaskClient ctx,
        string applicationId,
        string interactionToken,
        string channelId)
    {
        var input = new DeleteQueueOrchestratorInput(applicationId, interactionToken, channelId);
        await ctx.ScheduleNewOrchestrationInstanceAsync(nameof(DeleteQueueOrchestrator), input);
    }
}
