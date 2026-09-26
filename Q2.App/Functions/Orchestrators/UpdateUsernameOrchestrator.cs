using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Microsoft.DurableTask.Entities;
using Q2.App.Common;
using Q2.App.Functions.Activities;
using Q2.App.Functions.Entities;

namespace Q2.App.Functions.Orchestrators;

public record UpdateUsernameOrchestratorInput(
    string ApplicationId,
    string InteractionToken,
    string PlayerId,
    string Username);

public static class UpdateUsernameOrchestrator
{
    [Function(nameof(UpdateUsernameOrchestrator))]
    public static async Task<bool> RunAsync(
        [OrchestrationTrigger] TaskOrchestrationContext ctx,
        UpdateUsernameOrchestratorInput input)
    {
        MessageInteractionCallback callback;
        try
        {
            var state = await ctx.PlayerSetIgnAsync(input.PlayerId, input.Username);
            callback = SuccessResponse(input.PlayerId, state.Ign);
        }
        catch (EntityOperationFailedException ex) when (ex.FailureDetails.IsCausedBy<PlayerDoesNotExistException>())
        {
            callback = ErrorResponse("You have not yet registered. Please use the `/register` command to register.");
        }

        return await ctx.EditOriginalInteractionResponseAsync(input.ApplicationId, input.InteractionToken, callback);
    }

    private static MessageInteractionCallback SuccessResponse(string userId, string username) => new(
        MessageFlags.IsComponentsV2,
        [
            new ContainerComponent([
                new TextDisplayComponent($"<@{userId}> you have successfully updated your username to **{username}**."),
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

public static class UpdateUsernameOrchestratorExtensions
{
    public static async Task ScheduleNewUpdateUsernameOrchestratorInstanceAsync(
        this DurableTaskClient ctx,
        string applicationId,
        string interactionToken,
        string playerId,
        string username)
    {
        var input = new UpdateUsernameOrchestratorInput(applicationId, interactionToken, playerId, username);
        await ctx.ScheduleNewOrchestrationInstanceAsync(nameof(UpdateUsernameOrchestrator), input);
    }
}
