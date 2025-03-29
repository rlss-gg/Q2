namespace Q2.Domain

open System

type PlayerSettings = {
    QueueNotificationsEnabled: bool
}

type Player = {
    Id: string
    Username: string
    PreviousUsernames: string list
    Servers: GameServer list
    Ranks: Map<GameMode, GameRank>
    Elo: int option
    LastRankUpdateAt: DateTime option
    Settings: PlayerSettings
}

module Player =
    let create id username =
        {
            Id = id
            Username = username
            PreviousUsernames = []
            Servers = []
            Ranks = Map.empty
            Elo = None
            LastRankUpdateAt = None
            Settings = {
                QueueNotificationsEnabled = true
            }
        }

    let setUsername username player =
        match username with
        | username when username = player.Username -> player
        | username ->
            { player with
                Username = username
                PreviousUsernames = player.PreviousUsernames @ [player.Username] }

    let setServers servers player =
        { player with Servers = servers }

    let addServer server player =
        setServers (player.Servers @ [server] |> List.distinct) player

    let removeServer server player =
        setServers (player.Servers |> List.filter ((<>) server)) player

    let setRanks ranks currentTime player =
        { player with
            Ranks = ranks
            LastRankUpdateAt = Some currentTime }

    let addRank mode rank currentTime player =
        setRanks (player.Ranks |> Map.add mode rank) currentTime player

    let removeRank mode currentTime player =
        setRanks (player.Ranks |> Map.remove mode) currentTime player

    let setElo elo player =
        { player with Elo = Some elo }

    let setQueueNotifications enabled player =
        let settings = { player.Settings with QueueNotificationsEnabled = enabled }
        { player with Settings = settings }
