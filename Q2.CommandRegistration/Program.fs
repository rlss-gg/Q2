open FSharp.Discord.Commands
open FSharp.Discord.Rest
open Microsoft.Extensions.DependencyInjection
open Q2.Presentation
open System
open System.Net.Http

let commands = [
    Command.ChatInput RegisterPlayerCommand.Metadata.Command
]

let globalCommands =
    commands
    |> List.map Command.toPayload
    |> List.choose (function | CommandPayload.Global c -> Some c | _ -> None)

let httpClientFactory =
    ServiceCollection()
    |> _.AddHttpClient()
    |> _.BuildServiceProvider()
    |> _.GetService<IHttpClientFactory>()

[<EntryPoint>]
let main argv =
    let id, token =
        match List.ofArray argv with
        | [id; token] -> id, token
        | _ -> failwith "Expected two arguments: Application ID and bot token"

    let client = httpClientFactory.CreateBotClient token
        
    let res, _ =
        client
        |> Rest.bulkOverwriteGlobalApplicationCommands (
            BulkOverwriteGlobalApplicationCommandsRequest(
                id,
                BulkOverwriteGlobalApplicationCommandsPayload(globalCommands)
            )
        )
        |> Async.AwaitTask
        |> Async.RunSynchronously

    match res with
    | Error (DiscordApiError.ClientError err) ->
        Console.Error.WriteLine $"{int err.Code} - ${err.Message}"
        failwith "Failed to register global application commands"

    | Error (DiscordApiError.RateLimit err) ->
        failwith "You are being rate limited"

    | Error (DiscordApiError.Serialization err) ->
        Console.Error.WriteLine $"Serialization error: {err}"
        failwith "Failed to serialize request"

    | Error (DiscordApiError.Unexcepted err) ->
        Console.Error.WriteLine $"Status code: {err}"
        failwith "Unexpected status code received"
        
    | Ok _ ->
        Console.WriteLine "Successfully registered global application commands!"
        0
