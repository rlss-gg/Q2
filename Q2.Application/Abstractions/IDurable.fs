namespace Q2.Application

open Q2.Domain
open System.Threading.Tasks

type IQueueDurable =
    abstract Create: queueId: string -> size: int -> unit
    abstract Get: queueId: string -> Task<Queue option>

    abstract AddEntrant: queueId: string -> playerId: string -> unit
    abstract RemoveEntrant: queueId: string -> playerId: string -> unit
    abstract SetEntrantReady: queueId: string -> playerId: string -> unit

type IMatchDurable =
    abstract Create: matchId: string -> team1: MatchTeam -> team2: MatchTeam -> unit
    abstract Get: matchId: string -> Task<Match option>

    abstract Report: matchId: string -> score1: int -> score2: int -> unit
    abstract Abandon: matchId: string -> unit

type IDurable =
    abstract Queues: IQueueDurable
    abstract Matches: IMatchDurable
