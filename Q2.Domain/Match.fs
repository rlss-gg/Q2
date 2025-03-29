namespace Q2.Domain

open System

type MatchTeam = {
    Players: string list
}

module MatchTeam =
    let create players =
        {
            Players = players
        }

type Match = {
    Id: string
    Teams: MatchTeam * MatchTeam
    CreatedAt: DateTime
}

module Match =
    let create id team1 team2 currentTime =
        {
            Id = id
            Teams = team1, team2
            CreatedAt = currentTime
        }
