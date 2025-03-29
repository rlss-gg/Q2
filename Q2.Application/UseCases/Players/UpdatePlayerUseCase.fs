module Q2.Application.UpdatePlayerUseCase

open Q2.Domain
open System.Threading.Tasks

type UpdatePlayerUseCaseRequest = {
    Id: string
    Username: string option
    Servers: GameServer list option
    Ranks: Map<GameMode, GameRank> option
    QueueNotifications: bool option
}

[<RequireQualifiedAccess>]
type UpdatePlayerUseCaseError =
    | PlayerDoesNotExist

let invoke username servers ranks currentTime queueNotifications player =
    player
    |> Option.foldBack (fun username player -> Player.setUsername username player) username
    |> Option.foldBack (fun servers player -> Player.setServers servers player) servers
    |> Option.foldBack (fun ranks player -> Player.setRanks ranks currentTime player) ranks
    |> Option.foldBack (fun notifications player -> Player.setQueueNotifications notifications player) queueNotifications

let run (env: #IPersistence & #ITime) (req: UpdatePlayerUseCaseRequest) = task {
    let currentTime = env.GetCurrentTime()

    match! env.Players.Get req.Id with
    | None ->
        return Error UpdatePlayerUseCaseError.PlayerDoesNotExist

    | Some player ->
        let res = invoke req.Username req.Servers req.Ranks currentTime req.QueueNotifications player
        return! env.Players.Set res |> Task.map Ok
}
