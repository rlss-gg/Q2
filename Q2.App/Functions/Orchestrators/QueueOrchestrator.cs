using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Microsoft.DurableTask.Entities;
using Q2.App.Common;
using Q2.App.Functions.Activities;
using Q2.App.Functions.Entities;

namespace Q2.App.Functions.Orchestrators;

public record QueueOrchestratorInput(string ApplicationId, string InteractionToken, string ChannelId, string PlayerId);

public static class QueueOrchestrator
{
    [Function(nameof(QueueOrchestrator))]
    public static async Task<bool> RunAsync(
        [OrchestrationTrigger] TaskOrchestrationContext ctx,
        QueueOrchestratorInput input)
    {
        await using (await ctx.Entities.LockEntitiesAsync([PlayerEntity.Id(input.PlayerId), QueueEntity.Id(input.ChannelId)]))
        {
            var player = await ctx.PlayerGetStateAsync(input.PlayerId);

            if (player is null)
            {
                return await ctx.EditOriginalInteractionResponseAsync(
                    input.ApplicationId,
                    input.InteractionToken,
                    ErrorResponse("You need to `/register` before you can join a Q2 queue."));
            }
            else if (!player.CanQueue())
            {
                return await ctx.EditOriginalInteractionResponseAsync(
                    input.ApplicationId,
                    input.InteractionToken,
                    ErrorResponse("You cannot queue while already in another queue or match."));
            }

            try
            {
                var state = await ctx.QueueEnqueueAsync(input.ChannelId, input.PlayerId);
                await ctx.PlayerJoinQueueAsync(input.PlayerId, input.ChannelId);

                var callback = SuccessResponse(input.PlayerId, state.Players.Count, state.Size);
                await ctx.EditOriginalInteractionResponseAsync(input.ApplicationId, input.InteractionToken, callback);

                if (state.Players.Count == state.Size)
                {
                    var matchId = ctx.NewGuid().ToString(); // TODO: Nice match ID

                    await ctx.CreateMatchAsync(
                        matchId,
                        input.ApplicationId,
                        input.InteractionToken,
                        input.ChannelId,
                        state.Players);
                }

                return true;
            }
            catch (EntityOperationFailedException ex) when (ex.FailureDetails.IsCausedBy<InvalidOperationException>())
            {
                return await ctx.EditOriginalInteractionResponseAsync(
                    input.ApplicationId,
                    input.InteractionToken,
                    ErrorResponse(ex.FailureDetails.ErrorMessage));
            }
        }
    }

    private static MessageInteractionCallback SuccessResponse(string userId, int count, int size) => new(
        MessageFlags.IsComponentsV2,
        [
            new ContainerComponent([
                new TextDisplayComponent($"<@{userId}> has joined the queue! There are currently **{count}/{size}** in the queue."),
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

public static class QueueOrchestratorExtensions
{
    public static async Task ScheduleNewQueueOrchestratorInstanceAsync(
        this DurableTaskClient ctx,
        string applicationId,
        string interactionToken,
        string channelId,
        string playerId)
    {
        var input = new QueueOrchestratorInput(applicationId, interactionToken, channelId, playerId);
        await ctx.ScheduleNewOrchestrationInstanceAsync(nameof(QueueOrchestrator), input);
    }
}
