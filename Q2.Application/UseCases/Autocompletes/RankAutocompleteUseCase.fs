namespace Q2.Application

open Q2.Domain

type RankAutocompleteUseCase = {
    Query: string
}

module RankAutocompleteUseCase =
    let run (req: RankAutocompleteUseCase) =
        GameRank.Serialization.values
        |> List.filter (fun v -> v.ToLower().Contains(req.Query.ToLower()))
        |> List.truncate 25
