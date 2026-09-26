using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using Q2.App.Common;

namespace Q2.App.Functions.Activities;

public record CreateFollowUpMessageActivityInput(
    string ApplicationId,
    string InteractionToken,
    MessageInteractionCallback Callback);

public static class CreateFollowUpMessageActivity
{
    [Function(nameof(CreateFollowUpMessageActivity))]
    public static async Task<bool> RunAsync([ActivityTrigger] CreateFollowUpMessageActivityInput input)
    {
        try
        {
            using var client = new DiscordClient();
            await client.CreateFollowUpMessageAsync(input.ApplicationId, input.InteractionToken, input.Callback);

            return true;
        }
        catch
        {
            return false;
        }
    }
}

public static class CreateFollowUpMessageActivityExtensions
{
    public static async Task<bool> CreateFollowUpMessageAsync(
        this TaskOrchestrationContext ctx,
        string applicationId,
        string interactionToken,
        MessageInteractionCallback callback)
    {
        return await ctx.CallActivityAsync<bool>(
            nameof(CreateFollowUpMessageActivity),
            new CreateFollowUpMessageActivityInput(applicationId, interactionToken, callback));
    }
}
