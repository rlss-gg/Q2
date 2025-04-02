namespace Q2.AppHost

open Q2.Application
open Q2.Infrastructure.Persistence
open System

type PlayerPersistence(comsos) =
    interface IPlayerPersistence with
        member _.Get playerId = comsos |> Cosmos.Player.get playerId
        member _.Set player = comsos |> Cosmos.Player.set player

type RankPersistence(comsos) =
    interface IRankPersistence with
        member _.Get rankId = comsos |> Cosmos.Rank.get rankId
        member _.Set rank = comsos |> Cosmos.Rank.set rank
        
type GuildPersistence(comsos) =
    interface IGuildPersistence with
        member _.Get guildId = comsos |> Cosmos.Guild.get guildId
        member _.Set guild = comsos |> Cosmos.Guild.set guild
        
type QueuePersistence(comsos) =
    interface IQueuePersistence with
        member _.Get queueId = comsos |> Cosmos.Queue.get queueId
        member _.Set queue = comsos |> Cosmos.Queue.set queue

type MatchPersistence(comsos) =
    interface IMatchPersistence with
        member _.Get matchId = comsos |> Cosmos.Match.get matchId
        member _.Set match' = comsos |> Cosmos.Match.set match'

type Env (cosmos) =
    interface IEnv
    
    interface IPersistence with
        member _.Players = PlayerPersistence cosmos
        member _.Ranks = RankPersistence cosmos
        member _.Guilds = GuildPersistence cosmos
        member _.Queues = QueuePersistence cosmos
        member _.Matches = MatchPersistence cosmos

    interface ITime with
        member _.GetCurrentTime () = DateTime.UtcNow
