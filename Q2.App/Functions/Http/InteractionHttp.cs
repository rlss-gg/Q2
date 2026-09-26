using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask.Client;
using Microsoft.Win32;
using NSec.Cryptography;
using Q2.App.Common;
using Q2.App.Functions.Entities;
using Q2.App.Functions.Orchestrators;
using System.Net;
using System.Text;
using System.Text.Json;

namespace Q2.App.Functions.Http;

public static class InteractionHttp
{
    private static readonly string _publicKey = Environment.GetEnvironmentVariable("DISCORD_PUBLIC_KEY")
        ?? throw new InvalidOperationException("Missing 'DISCORD_PUBLIC_KEY' environment varialble");

    [Function(nameof(InteractionHttp))]
    public static async Task<HttpResponseData> RunAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "interaction")] HttpRequestData req,
        [DurableClient] DurableTaskClient durableClient)
    {
        if (!req.Headers.TryGetValues("X-Signature-Ed25519", out var signatures)
            || !req.Headers.TryGetValues("X-Signature-Timestamp", out var timestamps))
        {
            return req.CreateResponse(HttpStatusCode.Unauthorized);
        }

        var signature = signatures.First();
        var timestamp = timestamps.First();
        var content = await req.ReadAsStringAsync() ?? "";

        var messageBytes = Encoding.UTF8.GetBytes(timestamp + content);
        var signatureBytes = Convert.FromHexString(signature);
        var publicKeyBytes = Convert.FromHexString(_publicKey);

        var publicKey = PublicKey.Import(SignatureAlgorithm.Ed25519, publicKeyBytes, KeyBlobFormat.RawPublicKey);

        if (!SignatureAlgorithm.Ed25519.Verify(publicKey, messageBytes, signatureBytes))
            return req.CreateResponse(HttpStatusCode.Unauthorized);

        var interaction = JsonSerializer.Deserialize(content, InteractionContext.Default.Interaction);

        if (interaction is PingInteraction)
        {
            return await CreatePingResponse(req);
        }
        else if (interaction is ApplicationCommandInteraction { Data.Name: "manage" } manage)
        {
            if (manage.Data.Options.FirstOrDefault() is SubCommandGroupApplicationCommandInteractionDataOption { Name: "queues" } queues)
            {
                if (queues.Options.FirstOrDefault() is SubCommandApplicationCommandInteractionDataOption { Name: "create" } create)
                {
                    if (create.Options.FirstOrDefault() is IntegerApplicationCommandInteractionDataOption size)
                    {
                        await durableClient.ScheduleNewCreateQueueOrchestratorInstanceAsync(
                            manage.ApplicationId,
                            manage.Token,
                            manage.ChannelId,
                            size.Value);

                        return await CreateDeferredChannelMessageWithSourceResponse(req);
                    }
                    else
                    {
                        return req.CreateResponse(HttpStatusCode.BadRequest);
                    }
                }
                else if (queues.Options.FirstOrDefault() is SubCommandApplicationCommandInteractionDataOption { Name: "delete" } delete)
                {
                    await durableClient.ScheduleNewDeleteQueueOrchestratorInstanceAsync(
                        manage.ApplicationId,
                        manage.Token,
                        manage.ChannelId);

                    return await CreateDeferredChannelMessageWithSourceResponse(req);
                }
                else
                {
                    return req.CreateResponse(HttpStatusCode.BadRequest);
                }
            }
            else if (manage.Data.Options.FirstOrDefault() is SubCommandGroupApplicationCommandInteractionDataOption { Name: "players" } players)
            {
                if (players.Options.FirstOrDefault() is SubCommandApplicationCommandInteractionDataOption { Name: "delete" } delete)
                {
                    var playerId = delete.Options
                        .SelectMany(o => o is UserApplicationCommandInteractionDataOption { Name: "player" } player
                            ? [player.Value]
                            : new List<string>())
                        .FirstOrDefault();

                    if (playerId is not null)
                    {
                        await durableClient.ScheduleNewDeletePlayerOrchestratorInstanceAsync(
                            manage.ApplicationId,
                            manage.Token,
                            manage.Member.User.Id,
                            playerId);

                        return await CreateDeferredChannelMessageWithSourceResponse(req);
                    }
                    else
                    {
                        return req.CreateResponse(HttpStatusCode.BadRequest);
                    }
                }
                else
                {
                    return req.CreateResponse(HttpStatusCode.BadRequest);
                }
            }
            else
            {
                return req.CreateResponse(HttpStatusCode.BadRequest);
            }
        }
        else if (interaction is ApplicationCommandInteraction { Data.Name: "register" } register)
        {
            var username = register.Data.Options
                .SelectMany(o => o is StringApplicationCommandInteractionDataOption { Name: "username" } username
                    ? [username.Value]
                    : new List<string>())
                .FirstOrDefault();

            var rankString = register.Data.Options
                .SelectMany(o => o is StringApplicationCommandInteractionDataOption { Name: "rank" } rank
                    ? [rank.Value]
                    : new List<string>())
                .FirstOrDefault();

            Rank? rank = rankString switch
            {
                "Bronze" => Rank.Bronze,
                "Silver" => Rank.Silver,
                "Gold" => Rank.Gold,
                "Platinum" => Rank.Platinum,
                "Diamond" => Rank.Diamond,
                "Champion" => Rank.Champion,
                "Grand Champion" => Rank.GrandChampion,
                _ => null,
            };

            if (username is not null && rank is not null)
            {
                await durableClient.ScheduleNewRegisterOrchestratorInstanceAsync(
                    register.ApplicationId,
                    register.Token,
                    register.Member.User.Id,
                    username,
                    (Rank)rank);

                return await CreateDeferredChannelMessageWithSourceResponse(req);
            }
            else
            {
                return req.CreateResponse(HttpStatusCode.BadRequest);
            }
        }
        else if (interaction is ApplicationCommandInteraction { Data.Name: "update-username" } updateUsername)
        {
            var username = updateUsername.Data.Options
                .SelectMany(o => o is StringApplicationCommandInteractionDataOption { Name: "username" } username
                    ? [username.Value]
                    : new List<string>())
                .FirstOrDefault();

            if (username is not null)
            {
                await durableClient.ScheduleNewUpdateUsernameOrchestratorInstanceAsync(
                    updateUsername.ApplicationId,
                    updateUsername.Token,
                    updateUsername.Member.User.Id,
                    username);

                return await CreateDeferredChannelMessageWithSourceResponse(req);
            }
            else
            {
                return req.CreateResponse(HttpStatusCode.BadRequest);
            }
        }
        else if (interaction is ApplicationCommandInteraction { Data.Name: "status" } status)
        {
            await durableClient.ScheduleNewStatusOrchestratorInstanceAsync(
                status.ApplicationId,
                status.Token,
                status.ChannelId,
                status.Member.User.Id);

            return await CreateDeferredChannelMessageWithSourceResponse(req);
        }
        else if (interaction is ApplicationCommandInteraction { Data.Name: "queue" } queue)
        {
            await durableClient.ScheduleNewQueueOrchestratorInstanceAsync(
                queue.ApplicationId,
                queue.Token,
                queue.ChannelId,
                queue.Member.User.Id);

            return await CreateDeferredChannelMessageWithSourceResponse(req);
        }
        else if (interaction is ApplicationCommandInteraction { Data.Name: "leave" } leave)
        {
            await durableClient.ScheduleNewLeaveOrchestratorInstanceAsync(
                leave.ApplicationId,
                leave.Token,
                leave.ChannelId,
                leave.Member.User.Id);

            return await CreateDeferredChannelMessageWithSourceResponse(req);
        }
        else if (interaction is ApplicationCommandInteraction { Data.Name: "report" } report)
        {
            var you = report.Data.Options
                .SelectMany(o => o is IntegerApplicationCommandInteractionDataOption { Name: "you" } you
                    ? [you.Value]
                    : new List<int?>())
                .FirstOrDefault();

            var opponent = report.Data.Options
                .SelectMany(o => o is IntegerApplicationCommandInteractionDataOption { Name: "opponent" } opponent
                    ? [opponent.Value]
                    : new List<int?>())
                .FirstOrDefault();

            if (you is not null && opponent is not null)
            {
                await durableClient.ScheduleNewReportOrchestratorInstanceAsync(
                    report.ApplicationId,
                    report.Token,
                    report.Member.User.Id,
                    (int)you,
                    (int)opponent);

                return await CreateDeferredChannelMessageWithSourceResponse(req);
            }
            else
            {
                return req.CreateResponse(HttpStatusCode.BadRequest);
            }
        }
        else if (interaction is MessageComponentInteraction mci)
        {
            if (mci.Data.CustomId.StartsWith("ready-"))
            {
                var matchId = mci.Data.CustomId["ready-".Length..];

                await durableClient.RaiseEventAsync(
                    MatchOrchestrator.InstanceId(matchId),
                    nameof(MatchOrchestratorReadyEvent),
                    new MatchOrchestratorReadyEvent(mci.Token, mci.Member.User.Id));

                return await CreateDeferredUpdateMessageResponse(req);
            }
            else
            {
                return req.CreateResponse(HttpStatusCode.BadRequest);
            }
        }
        else
        {
            return req.CreateResponse(HttpStatusCode.BadRequest);
        }
    }

    private static async Task<HttpResponseData> CreatePingResponse(HttpRequestData req)
    {
        var res = req.CreateResponse(HttpStatusCode.OK);

        var json = JsonSerializer.Serialize(new PongInteractionResponse(), InteractionResponseContext.Default.InteractionResponse);
        await res.WriteStringAsync(json);
        res.Headers.Add("Content-Type", "application/json");

        return res;
    }

    private static async Task<HttpResponseData> CreateDeferredChannelMessageWithSourceResponse(HttpRequestData req)
    {
        var interactionResponse = new DeferredChannelMessageWithSourceInteractionResponse();

        var res = req.CreateResponse(HttpStatusCode.OK);

        var json = JsonSerializer.Serialize(interactionResponse, InteractionResponseContext.Default.InteractionResponse);
        await res.WriteStringAsync(json);
        res.Headers.Add("Content-Type", "application/json");

        return res;
    }

    public static async Task<HttpResponseData> CreateDeferredUpdateMessageResponse(HttpRequestData req)
    {
        var interactionResponse = new DeferredUpdateMessageInteractionResponse();

        var res = req.CreateResponse(HttpStatusCode.OK);

        var json = JsonSerializer.Serialize(interactionResponse, InteractionResponseContext.Default.InteractionResponse);
        await res.WriteStringAsync(json);
        res.Headers.Add("Content-Type", "application/json");

        return res;
    }
}
