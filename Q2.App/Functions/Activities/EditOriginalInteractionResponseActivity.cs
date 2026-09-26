using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using Microsoft.Extensions.Logging;
using Q2.App.Common;

namespace Q2.App.Functions.Activities;

public record EditOriginalInteractionResponseActivityInput(
    string ApplicationId,
    string InteractionToken,
    MessageInteractionCallback Callback);

public static class EditOriginalInteractionResponseActivity
{
    [Function(nameof(EditOriginalInteractionResponseActivity))]
    public static async Task<bool> RunAsync([ActivityTrigger] EditOriginalInteractionResponseActivityInput input)
    {
        try
        {
            using var client = new DiscordClient();
            await client.EditOriginalInteractionResponseAsync(input.ApplicationId, input.InteractionToken, input.Callback);

            return true;
        }
        catch
        {
            return false;
        }
    }
}

public static class EditOriginalInteractionResponseActivityExtensions
{
    public static async Task<bool> EditOriginalInteractionResponseAsync(
        this TaskOrchestrationContext ctx,
        string applicationId,
        string interactionToken,
        MessageInteractionCallback callback)
    {
    try
        {
            return await ctx.CallActivityAsync<bool>(
                nameof(EditOriginalInteractionResponseActivity),
                new EditOriginalInteractionResponseActivityInput(applicationId, interactionToken, callback));
        }
        catch
        {
            throw;
        }
    }
}
