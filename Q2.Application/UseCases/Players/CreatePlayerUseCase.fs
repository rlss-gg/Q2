module Q2.Application.CreatePlayerUseCase

open Q2.Domain
open System.Threading.Tasks

type CreatePlayerUseCaseRequest = {
    Id: string
    Username: string
}

[<RequireQualifiedAccess>]
type CreatePlayerUseCaseError =
    | PlayerAlreadyExists

let invoke id username =
    Player.create id username

let run (env: #IPersistence) (req: CreatePlayerUseCaseRequest) = task {
    match! env.GetPlayer req.Id with
    | Some _ ->
        return Error CreatePlayerUseCaseError.PlayerAlreadyExists

    | None ->
        let res = invoke req.Id req.Username
        return! env.SetPlayer res |> Task.map Ok
}
