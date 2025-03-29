module Q2.Application.CreateGuildUseCase

open Q2.Domain
open System.Threading.Tasks

type CreateGuildUseCaseRequest = {
    Id: string
}

[<RequireQualifiedAccess>]
type CreateGuildUseCaseError =
    | GuildAlreadyExists

let invoke id =
    Guild.create id

let run (env: #IPersistence) (req: CreateGuildUseCaseRequest) = task {
    match! env.Guilds.Get req.Id with
    | Some _ ->
        return Error CreateGuildUseCaseError.GuildAlreadyExists

    | None ->
        let res = invoke req.Id
        return! env.Guilds.Set res |> Task.map Ok
}
