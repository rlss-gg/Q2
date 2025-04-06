namespace Q2.Application

type GetPlayerUseCase = {
    Id: string
}

[<RequireQualifiedAccess>]
type GetPlayerUseCaseError =
    | PlayerNotFound

module GetPlayerUseCase =
    let run (env: #IPersistence) (req: GetPlayerUseCase) = task {
        match! env.Players.Get req.Id with
        | None -> return Error GetPlayerUseCaseError.PlayerNotFound
        | Some player -> return Ok player
    }
