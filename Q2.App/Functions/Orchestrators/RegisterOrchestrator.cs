using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Microsoft.DurableTask.Entities;
using Q2.App.Common;
using Q2.App.Functions.Activities;
using Q2.App.Functions.Entities;

namespace Q2.App.Functions.Orchestrators;

public record RegisterOrchestratorInput(
    string ApplicationId,
    string InteractionToken,
    string PlayerId,
    string Username,
    Rank Rank);

public static class RegisterOrchestrator
{
    [Function(nameof(RegisterOrchestrator))]
    public static async Task<bool> RunAsync(
        [OrchestrationTrigger] TaskOrchestrationContext ctx,
        RegisterOrchestratorInput input)
    {
        MessageInteractionCallback callback;
        try
        {
            var state = await ctx.PlayerCreateAsync(input.PlayerId, input.Username, input.Rank);
            callback = SuccessResponse(input.PlayerId, state.Ign, state.Elo);
        }
        catch (EntityOperationFailedException ex) when (ex.FailureDetails.IsCausedBy<PlayerAlreadyExistsException>())
        {
            callback = ErrorResponse("You are already registered. If you want to change your username, use the `/update-username` command.");
        
            // TODO: Mention how to get ELO updated after rank-up (TBD)
        }

        return await ctx.EditOriginalInteractionResponseAsync(input.ApplicationId, input.InteractionToken, callback);
    }

    private static MessageInteractionCallback SuccessResponse(string userId, string username, int elo) => new(
        MessageFlags.IsComponentsV2,
        [
            new ContainerComponent([
                new TextDisplayComponent($"<@{userId}> you have successfully registered as **{username}** and based on your rank, your starting ELO is **{elo}**. You can now use `/queue` to play Q2.App."),
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

public static class RegisterOrchestratorExtensions
{
    public static async Task ScheduleNewRegisterOrchestratorInstanceAsync(
        this DurableTaskClient ctx,
        string applicationId,
        string interactionToken,
        string playerId,
        string username,
        Rank rank)
    {
        var input = new RegisterOrchestratorInput(applicationId, interactionToken, playerId, username, rank);
        await ctx.ScheduleNewOrchestrationInstanceAsync(nameof(RegisterOrchestrator), input);
    }
}
