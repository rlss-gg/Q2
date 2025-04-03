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
    let toGameServers (v: Region) =
        match v with
        | Region.NA -> [GameServer.USE; GameServer.USW; GameServer.USC]
        | Region.EU -> [GameServer.EU]
        | Region.ASIA -> [GameServer.ASC; GameServer.ASM; GameServer.IND]
        | Region.MENA -> [GameServer.ME]
        | Region.OCE -> [GameServer.OCE]
        | Region.SAF -> [GameServer.SAF]
        | Region.SAM -> [GameServer.SAM]

    let fromGameServer (v: GameServer) =
        match v with
        | GameServer.USE | GameServer.USW | GameServer.USC -> Region.NA
        | GameServer.EU -> Region.EU
        | GameServer.ASC | GameServer.ASM | GameServer.IND -> Region.ASIA
        | GameServer.ME -> Region.MENA
        | GameServer.OCE -> Region.OCE
        | GameServer.SAF -> Region.SAF
        | GameServer.SAM -> Region.SAM

    module Serialization =
        let [<Literal>] NA = "NA"
        let [<Literal>] EU = "EU"
        let [<Literal>] ASIA = "ASIA"
        let [<Literal>] MENA = "MENA"
        let [<Literal>] OCE = "OCE"
        let [<Literal>] SAF = "SAF"
        let [<Literal>] SAM = "SAM"

        let mapping =
            Map.empty
            |> Map.add Region.NA NA
            |> Map.add Region.EU EU
            |> Map.add Region.ASIA ASIA
            |> Map.add Region.MENA MENA
            |> Map.add Region.OCE OCE
            |> Map.add Region.SAF SAF
            |> Map.add Region.SAM SAM

        let keys =
            mapping |> Map.keys |> Seq.toList

        let values =
            mapping |> Map.values |> Seq.toList

    let toString (v: Region) =
        Serialization.mapping |> Map.pick (fun k s -> if k = v then Some s else None)

    let fromString (v: string) =
        Serialization.mapping |> Map.tryFindKey (fun _ s -> s = v)

    let (|Region|_|) (v: string) =
        fromString v
