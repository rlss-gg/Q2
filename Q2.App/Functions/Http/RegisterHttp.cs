using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;
using Q2.App.Common;

namespace Q2.App.Functions.Http;

public static class RegisterHttp
{
    private static readonly IList<ApplicationCommand> _commands = [
        new(
            "manage",
            "Manage various aspects of Q2",
            [
                new SubCommandGroupOption("queues", "Manage queues", [
                    new SubCommandOption("create", "Create a new queue in this channel", [
                        new IntegerOption("size", "The size of the queue", required: true, min: 2, max: 6)
                    ]),
                    new SubCommandOption("delete", "Delete the queue in this channel", []),
                ]),
                new SubCommandGroupOption("players", "Manage players", [
                    new SubCommandOption("delete", "Delete a player (bot administrator only)", [
                        new UserOption("player", "The player to delete", required: true),
                    ])
                ]),
            ],
            8,
            [IntegrationType.Guild],
            [ContextType.Guild]),

        new(
            "register",
            "Register to play Q2",
            [
                new StringOption("username", "Your in-game username", required: true, null),
                new StringOption("rank", "Your current in-game rank", required: true, [
                    new StringOptionChoice("Bronze", "Bronze"),
                    new StringOptionChoice("Silver", "Silver"),
                    new StringOptionChoice("Gold", "Gold"),
                    new StringOptionChoice("Platinum", "Platinum"),
                    new StringOptionChoice("Diamond", "Diamond"),
                    new StringOptionChoice("Champion", "Champion"),
                    new StringOptionChoice("Grand Champion", "Grand Champion"),
                ]),
            ],
            null,
            [IntegrationType.Guild],
            [ContextType.Guild]),

        new(
            "update-username",
            "Update your in-game username",
            [
                new StringOption("username", "Your in-game username", required: true, null),
            ],
            null,
            [IntegrationType.Guild],
            [ContextType.Guild]),

        new("status", "Check the current status of the queue", [], null, [IntegrationType.Guild], [ContextType.Guild]),

        new("queue", "Enter the channel queue", [], null, [IntegrationType.Guild], [ContextType.Guild]),

        new("leave", "Leave the channel queue", [], null, [IntegrationType.Guild], [ContextType.Guild]),

        new(
            "report",
            "Report the score of the match you are currently playing",
            [
                new IntegerOption("you", "Your score", required: true, min: 0, max: null),
                new IntegerOption("opponent", "Your opponent's score", required: true, min: 0, max: null),
            ],
            null,
            [IntegrationType.Guild],
            [ContextType.Guild]),
    ];

    [Function(nameof(RegisterHttp))]
    public static async Task<HttpResponseData> RunAsync(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "register")] HttpRequestData req)
    {
        using var client = new DiscordClient();

        var application = await client.GetCurrentApplicationAsync();
        await client.BulkOverwriteGlobalApplicationCommandsAsync(application.Id, _commands);

        return req.CreateResponse(HttpStatusCode.NoContent);
    }
}
