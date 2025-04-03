namespace Q2.Presentation

open FSharp.Discord.Commands
open FSharp.Discord.Types
open FSharp.Discord.Types.Serialization
open FSharp.Discord.Webhook
open Microsoft.Azure.Functions.Worker
open Microsoft.Azure.Functions.Worker.Http
open Microsoft.Extensions.Configuration
open Q2.Application
open System.Net
open Thoth.Json.Net

type InteractionController (configuration: IConfiguration, env: IEnv) =
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
            
        // Read interaction from body
        match Decode.fromString Interaction.decoder json with
        | Error _ -> return req.CreateResponse HttpStatusCode.BadRequest
        | Ok interaction ->

        // Handle interaction
        match interaction with
        | Ping ->
            let callback = { Type = InteractionCallbackType.PONG; Data = None }
            let body = Encode.toString 0 (InteractionResponse.encoder callback)
            
            let res = req.CreateResponse HttpStatusCode.OK
            do! res.WriteStringAsync body
            return res

        | interaction ->
            do! InteractionHandler.handle env interaction
            return req.CreateResponse HttpStatusCode.Accepted
    }
