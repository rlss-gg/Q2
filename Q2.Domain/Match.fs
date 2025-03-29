namespace Q2.Domain

open System

type MatchTeam = {
    Players: string list
    Score: int option
}

module MatchTeam =
    let create players =
        {
            Players = players
            Score = None
        }

    let setScore score team =
        { team with Score = score }

[<RequireQualifiedAccess>]
type MatchStatus =
    | InProgress
    | Completed of completedAt: DateTime
    | Abandoned of abandonedAt: DateTime

type Match = {
    Id: string
    Teams: MatchTeam * MatchTeam
    CreatedAt: DateTime
    Status: MatchStatus
}

module Match =
    let create id team1 team2 currentTime =
        {
            Id = id
            Teams = team1, team2
            CreatedAt = currentTime
            Status = MatchStatus.InProgress
        }

    let report score1 score2 currentTime match' =
        let teams =
            fst match'.Teams |> MatchTeam.setScore (Some score1),
            snd match'.Teams |> MatchTeam.setScore (Some score2)

        { match' with
            Teams = teams
            Status = MatchStatus.Completed currentTime }

    let abandon currentTime match' =
        { match' with Status = MatchStatus.Abandoned currentTime }
