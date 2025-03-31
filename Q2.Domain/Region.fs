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

    module Stringified =
        let [<Literal>] NA = "NA"
        let [<Literal>] EU = "EU"
        let [<Literal>] ASIA = "ASIA"
        let [<Literal>] MENA = "MENA"
        let [<Literal>] OCE = "OCE"
        let [<Literal>] SAF = "SAF"
        let [<Literal>] SAM = "SAM"

    let toString (v: Region) =
        match v with
        | Region.NA -> Stringified.NA
        | Region.EU -> Stringified.EU
        | Region.ASIA -> Stringified.ASIA
        | Region.MENA -> Stringified.MENA
        | Region.OCE -> Stringified.OCE
        | Region.SAF -> Stringified.SAF
        | Region.SAM -> Stringified.SAM

    let fromString (v: string) =
        match v with
        | Stringified.NA -> Some Region.NA
        | Stringified.EU -> Some Region.EU
        | Stringified.ASIA -> Some Region.ASIA
        | Stringified.MENA -> Some Region.MENA
        | Stringified.OCE -> Some Region.OCE
        | Stringified.SAF -> Some Region.SAF
        | Stringified.SAM -> Some Region.SAM
        | _ -> None
