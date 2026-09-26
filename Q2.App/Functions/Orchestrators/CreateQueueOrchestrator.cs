using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Microsoft.DurableTask.Entities;
using Q2.App.Common;
using Q2.App.Functions.Activities;
using Q2.App.Functions.Entities;

namespace Q2.App.Functions.Orchestrators;

public record CreateQueueOrchestratorInput(string ApplicationId, string InteractionToken, string ChannelId, int Size);

public static class CreateQueueOrchestrator
{
    [Function(nameof(CreateQueueOrchestrator))]
    public static async Task<bool> RunAsync(
        [OrchestrationTrigger] TaskOrchestrationContext ctx,
        CreateQueueOrchestratorInput input)
    {
        MessageInteractionCallback callback;
        try
        {
            var res = await ctx.QueueCreateAsync(input.ChannelId, input.Size);
            callback = SuccessResponse(input.Size);
        }
        catch (EntityOperationFailedException ex) when (ex.FailureDetails.IsCausedBy<InvalidOperationException>())
        {
            callback = ErrorResponse(ex.FailureDetails.ErrorMessage);
        }
        
        return await ctx.EditOriginalInteractionResponseAsync(input.ApplicationId, input.InteractionToken, callback);
    }

    private static MessageInteractionCallback SuccessResponse(int size) => new(
        MessageFlags.IsComponentsV2 | MessageFlags.Ephemeral,
        [
            new ContainerComponent([
                new TextDisplayComponent($"Created **{size/2}v{size/2}** queue."),
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

public static class CreateQueueOrchestratorExtensions
{
    public static async Task ScheduleNewCreateQueueOrchestratorInstanceAsync(
        this DurableTaskClient ctx,
        string applicationId,
        string interactionToken,
        string channelId,
        int size)
    {
        var input = new CreateQueueOrchestratorInput(applicationId, interactionToken, channelId, size);
        await ctx.ScheduleNewOrchestrationInstanceAsync(nameof(CreateQueueOrchestrator), input);
    }
}
