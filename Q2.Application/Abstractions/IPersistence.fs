namespace Q2.Application

open Q2.Domain
open System.Threading.Tasks

type IPlayerPersistence =
    abstract Get: playerId: string -> Task<Player option>
    abstract Set: player: Player -> Task<Player>

type IRankPersistence =
    abstract Get: rankId: string -> Task<Rank option>
    abstract Set: rank: Rank -> Task<Rank>

type IGuildPersistence =
    abstract Get: guildId: string -> Task<Guild option>
    abstract Set: guild: Guild -> Task<Guild>

type IQueuePersistence =
    abstract Get: queueId: string -> Task<Queue option>
    abstract Set: queue: Queue -> Task<Queue>

type IMatchPersistence =
    abstract Get: matchId: string -> Task<Match option>
    abstract Set: match': Match -> Task<Match>

type IPersistence =
    abstract Players: IPlayerPersistence
    abstract Ranks: IRankPersistence
    abstract Guilds: IGuildPersistence
    abstract Queues: IQueuePersistence
    abstract Matches: IMatchPersistence
