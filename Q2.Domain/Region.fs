namespace Q2.Domain

/// Available regions for Q2, combining nearby game GameServers
[<RequireQualifiedAccess>]
type Region =
    | NA
    | EU
    | ASIA
    | MENA
    | OCE
    | SAF
    | SAM

module Region =
    let fromGameServer (v: GameServer) =
        match v with
        | GameServer.USE | GameServer.USW | GameServer.USC -> Region.NA
        | GameServer.EU -> Region.EU
        | GameServer.ASC | GameServer.ASM | GameServer.IND -> Region.ASIA
        | GameServer.ME -> Region.MENA
        | GameServer.OCE -> Region.OCE
        | GameServer.SAF -> Region.SAF
        | GameServer.SAM -> Region.SAM
