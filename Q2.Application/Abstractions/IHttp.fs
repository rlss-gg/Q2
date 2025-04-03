namespace Q2.Application

open FSharp.Discord.Rest
open System.Net.Http

type IHttp =
    abstract HttpClient: unit -> HttpClient
    abstract BotClient: botToken: string -> IBotClient
    abstract OAuthClient: accessToken: string -> IOAuthClient
    abstract BasicClient: clientId: string -> clientSecret: string -> IBasicClient
