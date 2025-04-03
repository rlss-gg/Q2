namespace Q2.AppHost

open FSharp.Discord.Rest
open Microsoft.Extensions.Configuration
open Q2.Application
open Q2.Infrastructure.Persistence
open System
open System.Net.Http

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

type Env(httpClientFactory: IHttpClientFactory, configuration: IConfiguration, cosmos) =
    interface IEnv

    interface IHttp with
        member _.HttpClient () = httpClientFactory.CreateClient()
        member _.BotClient botToken = httpClientFactory.CreateBotClient botToken
        member _.OAuthClient accessToken = httpClientFactory.CreateOAuthClient accessToken
        member _.BasicClient clientId clientSecret = httpClientFactory.CreateBasicClient clientId clientSecret
    
    interface IPersistence with
        member _.Players = PlayerPersistence cosmos
        member _.Ranks = RankPersistence cosmos
        member _.Guilds = GuildPersistence cosmos
        member _.Queues = QueuePersistence cosmos
        member _.Matches = MatchPersistence cosmos

    interface ISecrets with
        member _.DiscordBotToken = configuration.GetValue<string> "DiscordBotToken"
        member _.DiscordClientId = configuration.GetValue<string> "DiscordClientId"
        member _.DiscordPublicKey = configuration.GetValue<string> "DiscordPublicKey"

    interface ITime with
        member _.GetCurrentTime () = DateTime.UtcNow
