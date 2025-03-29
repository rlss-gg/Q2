module Q2.Application.UpdateRankUseCase

open Q2.Domain
open System.Threading.Tasks

type UpdateRankUseCaseRequest = {
    Id: string
    Name: string option
    Criteria: RankCriteria list option
}

[<RequireQualifiedAccess>]
type UpdateRankUseCaseError =
    | RankDoesNotExist

let invoke name criteria rank =
    rank
    |> Option.foldBack (fun name rank -> Rank.setName name rank) name
    |> Option.foldBack (fun criteria rank -> Rank.setCriteria criteria rank) criteria

let run (env: #IPersistence) (req: UpdateRankUseCaseRequest) = task {
    match! env.GetRank req.Id with
    | None ->
        return Error UpdateRankUseCaseError.RankDoesNotExist

    | Some rank ->
        let res = invoke req.Name req.Criteria rank
        return! env.SetRank res |> Task.map Ok
}
