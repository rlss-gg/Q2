namespace Q2.Application

open Q2.Domain
open System.Threading.Tasks

type VerifyPlayerGcUseCase = {
    Id: string
}

[<RequireQualifiedAccess>]
type VerifyPlayerGcUseCaseError =
    | PlayerNotFound
    | PrimaryRankNotGc

module VerifyPlayerGcUseCase =
    let invoke currentTime player =
        player |> Player.verifyGc currentTime

    let run (env: #IPersistence & #ITime) (req: VerifyPlayerGcUseCase) = task {
        match! env.Players.Get req.Id with
        | None ->
            return Error VerifyPlayerGcUseCaseError.PlayerNotFound

        | Some player ->
            match player.Ranks.Primary with
            | Some (GameRank.GrandChampion _) ->
                let res = invoke (env.GetCurrentTime()) player
                return! env.Players.Set res |> Task.map Ok

            | _ ->
                return Error VerifyPlayerGcUseCaseError.PrimaryRankNotGc
    }
