namespace Q2.Domain

type Rank = {
    Id: string
    Name: string
    Public: bool
}

module Rank =
    let create id name public' =
        {
            Id = id
            Name = name
            Public = public'
        }

type RankProgression = {
    RankId: string
    PlacementElo: int
    MinimumElo: int option
    MaximumElo: int option
}

module RankProgression =
    let create id placementElo =
        {
            RankId = id
            PlacementElo = placementElo
            MinimumElo = None
            MaximumElo = None
        }

    let setMinimumElo elo progression =
        { progression with MinimumElo = elo }

    let setMaximumElo elo progression =
        { progression with MaximumElo = elo }
