namespace Q2.Presentation

open FSharp.Discord.Commands
open FSharp.Discord.Rest
open FSharp.Discord.Types
open FSharp.Discord.Types.Serialization
open FSharp.Discord.Webhook
open Microsoft.Azure.Functions.Worker
open Microsoft.Azure.Functions.Worker.Http
open Q2.Application
open System.Net
open System.Threading.Tasks
open Thoth.Json.Net

type InteractionController (env: IEnv) =
    [<Function "PostInteraction">]
    member _.Post (
        [<HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "interactions")>] req: HttpRequestData
    ) = task {
        let! json = req.ReadAsStringAsync()

        let signature =
            req.Headers.TryGetValues "x-signature-ed25519"
            |> fun (success, v) -> match success with | true -> Seq.tryHead v | false -> None

        let timestamp =
            req.Headers.TryGetValues "x-signature-timestamp"
            |> fun (success, v) -> match success with | true -> Seq.tryHead v | false -> None
            
        // Ensure headers are present
        match signature, timestamp with
        | None, None | None, Some _ | Some _, None -> return req.CreateResponse HttpStatusCode.Unauthorized
        | Some signature, Some timestamp ->

        // Validate signature
        match Ed25519.verify timestamp json signature env.DiscordPublicKey with
        | false -> return req.CreateResponse HttpStatusCode.Unauthorized
        | _ ->

        System.Console.WriteLine json // Temporary, for debugging
            
        // Read interaction from body
        match Decode.fromString Interaction.decoder json with
        | Error err ->
            System.Console.Error.WriteLine err // Temporary, for debugging
            return req.CreateResponse HttpStatusCode.BadRequest

        | Ok interaction ->
        
        // Handle interaction
        let client = env.BotClient env.DiscordBotToken

        let respond (response: CreateInteractionResponseRequest) = task {
            do! Rest.createInteractionResponse response client :> Task
            return req.CreateResponse HttpStatusCode.NoContent
        }

        match interaction with
        | Ping ->
            let res = req.CreateResponse HttpStatusCode.OK
            res.Headers.Add("Content-Type", "application/json")
            do! res.WriteStringAsync CommonResponse.ping
            return res
            
        | RegisterPlayerCommand.Validate action ->
            match action with
            | RegisterPlayerCommand.Action.InvalidArguments ->
                let response = CommonResponse.invalidArguments interaction.Id interaction.Token
                return! respond response
                
            | RegisterPlayerCommand.Action.RankAutocomplete query ->
                let choices = RankAutocompleteUseCase.run { Query = query }
                let response = RegisterPlayerResponse.rankAutocomplete interaction.Id interaction.Token choices
                return! respond response

            | RegisterPlayerCommand.Action.RunCommand (userId, username, region, rank) ->
                let data: RegisterPlayerUseCase = { Id = userId; Username = username; Region = region; Rank = rank }

                match! RegisterPlayerUseCase.run env data with
                | Error RegisterPlayerUseCaseError.PlayerAlreadyRegistered ->
                    let response = CommonResponse.failed interaction.Id interaction.Token
                    return! respond response

                | Ok player ->
                    let response = RegisterPlayerResponse.runCommand interaction.Id interaction.Token player
                    return! respond response

        | PlayerSettingsCommand.Validate action ->
            match action with
            | PlayerSettingsCommand.Action.InvalidArguments ->
                let response = CommonResponse.invalidArguments interaction.Id interaction.Token
                return! respond response

            | PlayerSettingsCommand.Action.ToggleNotifications userId ->
                let data: ToggleNotificationUseCase = { Id = userId }

                match! ToggleNotificationUseCase.run env data with
                | Error ToggleNotificationUseCaseError.PlayerNotFound ->
                    let response = CommonResponse.notRegistered interaction.Id interaction.Token
                    return! respond response

                | Ok player ->
                    let response = PlayerSettingsResponse.notifications interaction.Id interaction.Token player.Settings.QueueNotificationsEnabled
                    return! respond response

        | PlayerProfileCommand.Validate action ->
            match action with
            | PlayerProfileCommand.Action.InvalidArguments ->
                let response = CommonResponse.invalidArguments interaction.Id interaction.Token
                return! respond response

            | PlayerProfileCommand.Action.ViewProfile userId ->
                let data: GetPlayerUseCase = { Id = userId }

                match! GetPlayerUseCase.run env data with
                | Error GetPlayerUseCaseError.PlayerNotFound ->
                    let response = CommonResponse.notRegistered interaction.Id interaction.Token
                    return! respond response

                | Ok player ->
                    let response = PlayerProfileResponse.profile interaction.Id interaction.Token player
                    return! respond response

        | UserManagementCommand.Validate action ->
            match action with
            | UserManagementCommand.Action.InvalidArguments ->
                let response = CommonResponse.invalidArguments interaction.Id interaction.Token
                return! respond response

            | UserManagementCommand.Action.VerifyGc userId ->
                let data: VerifyPlayerGcUseCase = { Id = userId }

                match! VerifyPlayerGcUseCase.run env data with
                | Error VerifyPlayerGcUseCaseError.PlayerNotFound ->
                    let response = CommonResponse.notRegistered interaction.Id interaction.Token
                    return! respond response

                | Error VerifyPlayerGcUseCaseError.PrimaryRankNotGc ->
                    let response = UserManagementResponse.playerNotGc interaction.Id interaction.Token
                    return! respond response

                | Ok player ->
                    let response = UserManagementResponse.verifyGc interaction.Id interaction.Token player
                    return! respond response
        
        | _ ->
            return req.CreateResponse HttpStatusCode.BadRequest
    }
