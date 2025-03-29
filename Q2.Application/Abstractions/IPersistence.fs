namespace Q2.Application

open Q2.Domain
open System.Threading.Tasks

type IPersistence =
    abstract GetPlayer: playerId: string -> Task<Player option>
    abstract SetPlayer: player: Player -> Task<Player>

    abstract GetRank: rankId: string -> Task<Rank option>
    abstract SetRank: rank: Rank -> Task<Rank>
