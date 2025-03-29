module Q2.Application.RegisterPlayerUseCase

// - /register [ign] [region] [rank]

(*

// ----- REFERENCE USE CASE FROM OLD -----

module Q2.Application.CreateRankUseCase

open Q2.Domain
open System.Threading.Tasks

type CreateRankUseCaseRequest = {
    Id: string
    Name: string
    Origin: RankOrigin
    Criteria: RankCriteria
}

[<RequireQualifiedAccess>]
type CreateRankUseCaseError =
    | RankAlreadyExists

let invoke id name origin criteria =
    Rank.create id name origin criteria

let run (env: #IPersistence) (req: CreateRankUseCaseRequest) = task {
    match! env.Ranks.Get req.Id with
    | Some _ ->
        return Error CreateRankUseCaseError.RankAlreadyExists

    | None ->
        let res = invoke req.Id req.Name req.Origin req.Criteria
        return! env.Ranks.Set res |> Task.map Ok
}

*)
