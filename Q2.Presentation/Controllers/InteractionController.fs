namespace Q2.Presentation

open FSharp.Discord.Types.Serialization
open FSharp.Discord.Webhook
open Microsoft.Azure.Functions.Worker
open Microsoft.Azure.Functions.Worker.Http
open Q2.Application
open System.Net
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
            
        // Read interaction from body
        match Decode.fromString InteractionCreateEvent.decoder json, Decode.fromString Interaction.decoder json with
        | Ok event, Ok interaction ->
            match! InteractionHandler.handle env event interaction with
            | InteractionHandlerResponse.Ping ->
                let res = req.CreateResponse HttpStatusCode.OK
                res.Headers.Add("Content-Type", "application/json")
                do! res.WriteStringAsync CommonResponse.ping
                return res

            | InteractionHandlerResponse.Failure ->
                return req.CreateResponse HttpStatusCode.BadRequest

            | InteractionHandlerResponse.Success ->
                return req.CreateResponse HttpStatusCode.Accepted

        | _ ->
            return req.CreateResponse HttpStatusCode.BadRequest // TODO: Return generic error

        // TODO: Decode interaction metadata into event too rather than serializing both separately (FSharp.Discord change)
    }
