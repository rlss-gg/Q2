namespace Q2.Application

open Q2.Domain
open System.Threading.Tasks

type RegisterPlayerUseCase = {
    Id: string
    Username: string
    Region: Region
    Rank: GameRank
}

[<RequireQualifiedAccess>]
type RegisterPlayerUseCaseError =
    | PlayerAlreadyRegistered

module RegisterPlayerUseCase =
    let invoke id username region rank currentTime =
        Player.create id username
        |> Player.setRegion region
        |> Player.setPrimaryRank rank currentTime

    let run (env: #IPersistence & #ITime) (req: RegisterPlayerUseCase) = task {
        match! env.Players.Get req.Id with
        | Some _ ->
            return Error RegisterPlayerUseCaseError.PlayerAlreadyRegistered

        | None ->
            let res = invoke req.Id req.Username req.Region req.Rank (env.GetCurrentTime())
            return! env.Players.Set res |> Task.map Ok
    }
