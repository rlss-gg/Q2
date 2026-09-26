using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Q2.App.Common;
using Q2.App.Functions.Activities;
using Q2.App.Functions.Entities;

namespace Q2.App.Functions.Orchestrators;

public record ReportOrchestratorInput(
    string ApplicationId,
    string InteractionToken,
    string PlayerId,
    int PlayerScore,
    int OpponentScore);

public static class ReportOrchestrator
{
    [Function(nameof(ReportOrchestrator))]
    public static async Task<bool> RunAsync(
        [OrchestrationTrigger] TaskOrchestrationContext ctx,
        ReportOrchestratorInput input)
    {
        var state = await ctx.PlayerGetStateAsync(input.PlayerId);

        if (state?.CurrentMatchId is null)
        {
            await ctx.EditOriginalInteractionResponseAsync(
                input.ApplicationId,
                input.InteractionToken,
                ErrorResponse("You are not currently in a match."));
                
            return false;
        }

        if ((input.OpponentScore != 2 && input.PlayerScore != 2) || (input.OpponentScore == 2 && input.PlayerScore == 2))
        {
            await ctx.EditOriginalInteractionResponseAsync(
                input.ApplicationId,
                input.InteractionToken,
                ErrorResponse("The score for a best of 3 match must be one of **2-0**, **2-1**, **1-2**, or **0-2**."));

            return false;
        }

        var instanceId = MatchOrchestrator.InstanceId(state.CurrentMatchId);

        ctx.SendEvent(instanceId, nameof(MatchOrchestratorScoreReportEvent), new MatchOrchestratorScoreReportEvent(
            input.InteractionToken,
            input.PlayerId,
            input.PlayerScore,
            input.OpponentScore));

        // TODO: Move response back to this orchestrator?

        return true;
    }

    private static MessageInteractionCallback ErrorResponse(string message) => new(
        MessageFlags.IsComponentsV2 | MessageFlags.Ephemeral,
        [
            new ContainerComponent([
                new TextDisplayComponent($"Error: {message}"),
            ]),
        ]);
}

public static class ReportOrchestratorExtensions
{
    public static async Task ScheduleNewReportOrchestratorInstanceAsync(
        this DurableTaskClient ctx,
        string applicationId,
        string interactionToken,
        string playerId,
        int playerScore,
        int opponentScore)
    {
        var input = new ReportOrchestratorInput(applicationId, interactionToken, playerId, playerScore, opponentScore);
        await ctx.ScheduleNewOrchestrationInstanceAsync(nameof(ReportOrchestrator), input);
    }
}
