namespace Q2.Domain

open System
open System.Text.RegularExpressions

/// Rocket League Sideswipe competitive ranks
[<RequireQualifiedAccess>]
type GameRank =
    | BronzeI
    | BronzeII
    | BronzeIII
    | BronzeIV
    | BronzeV
    | SilverI
    | SilverII
    | SilverIII
    | SilverIV
    | SilverV
    | GoldI
    | GoldII
    | GoldIII
    | GoldIV
    | GoldV
    | PlatinumI
    | PlatinumII
    | PlatinumIII
    | PlatinumIV
    | PlatinumV
    | DiamondI
    | DiamondII
    | DiamondIII
    | DiamondIV
    | DiamondV
    | ChampionI
    | ChampionII
    | ChampionIII
    | ChampionIV
    | ChampionV
    | GrandChampion of elo: int option

module GameRank =
    module Serialization =
        let [<Literal>] BronzeI = "Bronze I"
        let [<Literal>] BronzeII = "Bronze II"
        let [<Literal>] BronzeIII = "Bronze III"
        let [<Literal>] BronzeIV = "Bronze IV"
        let [<Literal>] BronzeV = "Bronze V"
        let [<Literal>] SilverI = "Silver I"
        let [<Literal>] SilverII = "Silver II"
        let [<Literal>] SilverIII = "Silver III"
        let [<Literal>] SilverIV = "Silver IV"
        let [<Literal>] SilverV = "Silver V"
        let [<Literal>] GoldI = "Gold I"
        let [<Literal>] GoldII = "Gold II"
        let [<Literal>] GoldIII = "Gold III"
        let [<Literal>] GoldIV = "Gold IV"
        let [<Literal>] GoldV = "Gold V"
        let [<Literal>] PlatinumI = "Platinum I"
        let [<Literal>] PlatinumII = "Platinum II"
        let [<Literal>] PlatinumIII = "Platinum III"
        let [<Literal>] PlatinumIV = "Platinum IV"
        let [<Literal>] PlatinumV = "Platinum V"
        let [<Literal>] DiamondI = "Diamond I"
        let [<Literal>] DiamondII = "Diamond II"
        let [<Literal>] DiamondIII = "Diamond III"
        let [<Literal>] DiamondIV = "Diamond IV"
        let [<Literal>] DiamondV = "Diamond V"
        let [<Literal>] ChampionI = "Champion I"
        let [<Literal>] ChampionII = "Champion II"
        let [<Literal>] ChampionIII = "Champion III"
        let [<Literal>] ChampionIV = "Champion IV"
        let [<Literal>] ChampionV = "Champion V"
        let [<Literal>] GrandChampion = "Grand Champion"

        let grandChampionElo (elo: int) = $"Grand Champion ({elo})"

        let mapping =
            Map.empty
            |> Map.add GameRank.BronzeI BronzeI
            |> Map.add GameRank.BronzeII BronzeII
            |> Map.add GameRank.BronzeIII BronzeIII
            |> Map.add GameRank.BronzeIV BronzeIV
            |> Map.add GameRank.BronzeV BronzeV
            |> Map.add GameRank.SilverI SilverI
            |> Map.add GameRank.SilverII SilverII
            |> Map.add GameRank.SilverIII SilverIII
            |> Map.add GameRank.SilverIV SilverIV
            |> Map.add GameRank.SilverV SilverV
            |> Map.add GameRank.GoldI GoldI
            |> Map.add GameRank.GoldII GoldII
            |> Map.add GameRank.GoldIII GoldIII
            |> Map.add GameRank.GoldIV GoldIV
            |> Map.add GameRank.GoldV GoldV
            |> Map.add GameRank.PlatinumI PlatinumI
            |> Map.add GameRank.PlatinumII PlatinumII
            |> Map.add GameRank.PlatinumIII PlatinumIII
            |> Map.add GameRank.PlatinumIV PlatinumIV
            |> Map.add GameRank.PlatinumV PlatinumV
            |> Map.add GameRank.DiamondI DiamondI
            |> Map.add GameRank.DiamondII DiamondII
            |> Map.add GameRank.DiamondIII DiamondIII
            |> Map.add GameRank.DiamondIV DiamondIV
            |> Map.add GameRank.DiamondV DiamondV
            |> Map.add GameRank.ChampionI ChampionI
            |> Map.add GameRank.ChampionII ChampionII
            |> Map.add GameRank.ChampionIII ChampionIII
            |> Map.add GameRank.ChampionIV ChampionIV
            |> Map.add GameRank.ChampionV ChampionV
            |> Map.add (GameRank.GrandChampion None) GrandChampion

        let keys =
            mapping |> Map.keys |> Seq.toList

        let values =
            mapping |> Map.values |> Seq.toList

    let toString (v: GameRank) =
        match v with
        | GameRank.GrandChampion (Some elo) -> Serialization.grandChampionElo elo
        | rank -> Serialization.mapping |> Map.pick (fun k s -> if k = rank then Some s else None)

    let fromString (str: string) =
        let (|GrandChampionElo|_|) (str: string) =
            let regex = Regex "Grand Champion \((?<elo>\d+)\)"

            match regex.Match str with
            | m when m.Success ->
                match Int32.TryParse m.Groups["elo"].Value with
                | true, elo -> Some (GameRank.GrandChampion (Some elo))
                | _ -> None

            | _ -> None

        match str with
        | GrandChampionElo rank -> Some rank
        | v -> Serialization.mapping |> Map.tryFindKey (fun _ s -> s = v)

    let (|GameRank|_|) (str: string) =
        fromString str

/// Rocket League Sideswipe game modes
[<RequireQualifiedAccess>]
type GameMode =
    | Duel
    | Doubles
    | Threes
    | Hoops
    | Volleyball
    | Squash

module GameMode =
    module Serialization =
        let [<Literal>] Duel = "Duel"
        let [<Literal>] Doubles = "Doubles"
        let [<Literal>] Threes = "Threes"
        let [<Literal>] Hoops = "Hoops"
        let [<Literal>] Volleyball = "Volleyball"
        let [<Literal>] Squash = "Squash"

        let mapping =
            Map.empty
            |> Map.add GameMode.Duel Duel
            |> Map.add GameMode.Doubles Doubles
            |> Map.add GameMode.Threes Threes
            |> Map.add GameMode.Hoops Hoops
            |> Map.add GameMode.Volleyball Volleyball
            |> Map.add GameMode.Squash Squash

        let keys =
            mapping |> Map.keys |> Seq.toList

        let values =
            mapping |> Map.values |> Seq.toList
            
    let toString (v: GameMode) =
        Serialization.mapping |> Map.pick (fun k s -> if k = v then Some s else None)

    let fromString (v: string) =
        Serialization.mapping |> Map.tryFindKey (fun _ s -> s = v)

    let (|GameMode|_|) (str: string) =
        fromString str

/// Rocket League server locations according to RL Wiki https://rocketleague.fandom.com/wiki/Servers
[<RequireQualifiedAccess>]
type GameServer =
    | USE
    | USW
    | USC
    | EU
    | ASC
    | ASM
    | ME
    | OCE
    | SAF
    | SAM
    | IND

module GameServer =
    module Serialization =
        let [<Literal>] USE = "USE"
        let [<Literal>] USW = "USW"
        let [<Literal>] USC = "USC"
        let [<Literal>] EU = "EU"
        let [<Literal>] ASC = "ASC"
        let [<Literal>] ASM = "ASM"
        let [<Literal>] ME = "ME"
        let [<Literal>] OCE = "OCE"
        let [<Literal>] SAF = "SAF"
        let [<Literal>] SAM = "SAM"
        let [<Literal>] IND = "IND"
        
        let mapping =
            Map.empty
            |> Map.add GameServer.USE USE
            |> Map.add GameServer.USW USW
            |> Map.add GameServer.USC USC
            |> Map.add GameServer.EU EU
            |> Map.add GameServer.ASC ASC
            |> Map.add GameServer.ASM ASM
            |> Map.add GameServer.ME ME
            |> Map.add GameServer.OCE OCE
            |> Map.add GameServer.SAF SAF
            |> Map.add GameServer.SAM SAM
            |> Map.add GameServer.IND IND

        let keys =
            mapping |> Map.keys |> Seq.toList

        let values =
            mapping |> Map.values |> Seq.toList
            
    let toString (v: GameServer) =
        Serialization.mapping |> Map.pick (fun k s -> if k = v then Some s else None)

    let fromString (v: string) =
        Serialization.mapping |> Map.tryFindKey (fun _ s -> s = v)

    let (|GameServer|_|) (str: string) =
        fromString str
