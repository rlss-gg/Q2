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
    Rank: GameRank option
    RankUpdatedAt: DateTime option
    Settings: PlayerSettings
}

module Player =
    let create id username =
        {
            Id = id
            Username = username
            PreviousUsernames = []
            Servers = []
            Rank = None
            RankUpdatedAt = None
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

    let addServer server player =
        { player with Servers = player.Servers @ [server] |> List.distinct }

    let removeServer server player =
        { player with Servers = player.Servers |> List.filter ((<>) server) }

    let setGameRank rank currentTime player =
        { player with
            Rank = rank
            RankUpdatedAt = Some currentTime }

    let setQueueNotifications enabled player =
        let settings = { player.Settings with QueueNotificationsEnabled = enabled }
        { player with Settings = settings }
