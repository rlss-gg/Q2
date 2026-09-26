using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Microsoft.DurableTask.Entities;
using Q2.App.Common;
using Q2.App.Functions.Activities;
using Q2.App.Functions.Entities;

namespace Q2.App.Functions.Orchestrators;

public record DeletePlayerOrchestratorInput(string ApplicationId, string InteractionToken, string InvokerId, string PlayerId);

public static class DeletePlayerOrchestrator
{
    [Function(nameof(DeletePlayerOrchestrator))]
    public static async Task<bool> RunAsync(
        [OrchestrationTrigger] TaskOrchestrationContext ctx,
        DeletePlayerOrchestratorInput input)
    {
        // Only allow bot administrator
        if (input.InvokerId != "311358385591156736")
        {
            await ctx.EditOriginalInteractionResponseAsync(
                input.ApplicationId,
                input.InteractionToken,
                ErrorResponse("You do not have permission to use this command."));

            return false;
        }

        // Ensure player exists
        var state = await ctx.PlayerGetStateAsync(input.PlayerId);

        if (state is null)
        { 
            await ctx.EditOriginalInteractionResponseAsync(
                input.ApplicationId,
                input.InteractionToken,
                ErrorResponse("Player not found."));

            return false;
        }

        // Lock player and queue (if any) to prevent race conditions then delete
        var locks = new List<EntityInstanceId> { PlayerEntity.Id(input.PlayerId) };

        if (state.CurrentQueueId is not null)
            locks.Add(QueueEntity.Id(state.CurrentQueueId));

        await using (await ctx.Entities.LockEntitiesAsync(locks))
        {
            await ctx.PlayerDeleteAsync(input.PlayerId);

            if (state.CurrentQueueId is not null)
            {
                try
                {
                    await ctx.QueueDequeueAsync(state.CurrentQueueId, input.PlayerId);
                }
                catch
                {
                    // Ignore errors as none are critical to the player deletion
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
                new TextDisplayComponent($"Deleted player."),
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

public static class DeletePlayerOrchestratorExtensions
{
    public static async Task ScheduleNewDeletePlayerOrchestratorInstanceAsync(
        this DurableTaskClient ctx,
        string applicationId,
        string interactionToken,
        string invokerId,
        string playerId)
    {
        var input = new DeletePlayerOrchestratorInput(applicationId, interactionToken, invokerId, playerId);
        await ctx.ScheduleNewOrchestrationInstanceAsync(nameof(DeletePlayerOrchestrator), input);
    }
}
