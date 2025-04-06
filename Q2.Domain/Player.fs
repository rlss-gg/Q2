namespace Q2.Domain

open System

type PlayerSettings = {
    QueueNotificationsEnabled: bool
}

type PlayerRanks = {
    Primary: GameRank option
    Modes: Map<GameMode, GameRank>
    LastUpdateAt: DateTime option
    VerifiedGcAt: DateTime option
}

type Player = {
    Id: string
    Username: string
    PreviousUsernames: string list
    Servers: GameServer list
    Ranks: PlayerRanks
    Elo: int option
    Settings: PlayerSettings
}

module Player =
    let create id username =
        {
            Id = id
            Username = username
            PreviousUsernames = []
            Servers = []
            Ranks = {
                Primary = None
                Modes = Map.empty
                LastUpdateAt = None
                VerifiedGcAt = None
            }
            Elo = None
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

    let setRegion region player =
        { player with Servers = Region.toGameServers region }

    let setServers servers player =
        { player with Servers = servers }

    let addServer server player =
        setServers (player.Servers @ [server] |> List.distinct) player

    let removeServer server player =
        setServers (player.Servers |> List.filter ((<>) server)) player

    let setPrimaryRank rank currentTime player =
        let ranks =
            { player.Ranks with
                Primary = Some rank
                LastUpdateAt = Some currentTime }

        { player with Ranks = ranks }

    let removePrimaryRank currentTime player =
        let ranks =
            { player.Ranks with
                Primary = None
                LastUpdateAt = Some currentTime }

        { player with Ranks = ranks }

    let verifyGc currentTime player =
        let ranks = { player.Ranks with VerifiedGcAt = Some currentTime }
        { player with Ranks = ranks }

    let isVerifiedGc player =
        match player.Ranks.Primary, player.Ranks.VerifiedGcAt with
        | Some (GameRank.GrandChampion _), Some _ -> true
        | _ -> false

    let setRanks ranks currentTime player =
        let ranks =
            { player.Ranks with
                Modes = ranks
                LastUpdateAt = Some currentTime }

        { player with Ranks = ranks }

    let addRank mode rank currentTime player =
        setRanks (player.Ranks.Modes |> Map.add mode rank) currentTime player

    let removeRank mode currentTime player =
        setRanks (player.Ranks.Modes |> Map.remove mode) currentTime player

    let setElo elo player =
        { player with Elo = Some elo }

    let setQueueNotifications enabled player =
        let settings = { player.Settings with QueueNotificationsEnabled = enabled }
        { player with Settings = settings }
