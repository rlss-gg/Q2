namespace Q2.Domain

[<RequireQualifiedAccess>]
type RankOrigin =
    | Global
    | Guild of guildId: string

type GameRankCriteria = {
    Min: GameRank
    Max: GameRank
}

[<RequireQualifiedAccess>]
type EloRankCriteria =
    | Range of min: int * max: int
    | LowerBound of elo: int
    | UpperBound of elo: int

type RankCriteria = {
    Game: Map<GameMode, GameRankCriteria> option
    Elo: EloRankCriteria option
}

type Rank = {
    Id: string
    Name: string
    Origin: RankOrigin
    Criteria: RankCriteria list
}

module Rank =
    let create id name origin criteria =
        {
            Id = id
            Name = name
            Origin = origin
            Criteria = criteria
        }
        
    let setName name rank =
        { rank with Name = name }
        
    let setCriteria criteria rank =
        { rank with Criteria = criteria }
