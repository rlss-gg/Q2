namespace Q2.Application

open FSharp.Discord.Rest
open System.Net.Http

type ISecrets =
    abstract DiscordBotToken: string
    abstract DiscordClientId: string
    abstract DiscordPublicKey: string
