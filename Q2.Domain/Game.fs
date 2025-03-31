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
    module Stringified =
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

    let toString (v: GameRank) =
        match v with
        | GameRank.BronzeI -> Stringified.BronzeI
        | GameRank.BronzeII -> Stringified.BronzeII
        | GameRank.BronzeIII -> Stringified.BronzeIII
        | GameRank.BronzeIV -> Stringified.BronzeIV
        | GameRank.BronzeV -> Stringified.BronzeV
        | GameRank.SilverI -> Stringified.SilverI
        | GameRank.SilverII -> Stringified.SilverII
        | GameRank.SilverIII -> Stringified.SilverIII
        | GameRank.SilverIV -> Stringified.SilverIV
        | GameRank.SilverV -> Stringified.SilverV
        | GameRank.GoldI -> Stringified.GoldI
        | GameRank.GoldII -> Stringified.GoldII
        | GameRank.GoldIII -> Stringified.GoldIII
        | GameRank.GoldIV -> Stringified.GoldIV
        | GameRank.GoldV -> Stringified.GoldV
        | GameRank.PlatinumI -> Stringified.PlatinumI
        | GameRank.PlatinumII -> Stringified.PlatinumII
        | GameRank.PlatinumIII -> Stringified.PlatinumIII
        | GameRank.PlatinumIV -> Stringified.PlatinumIV
        | GameRank.PlatinumV -> Stringified.PlatinumV
        | GameRank.DiamondI -> Stringified.DiamondI
        | GameRank.DiamondII -> Stringified.DiamondII
        | GameRank.DiamondIII -> Stringified.DiamondIII
        | GameRank.DiamondIV -> Stringified.DiamondIV
        | GameRank.DiamondV -> Stringified.DiamondV
        | GameRank.ChampionI -> Stringified.ChampionI
        | GameRank.ChampionII -> Stringified.ChampionII
        | GameRank.ChampionIII -> Stringified.ChampionIII
        | GameRank.ChampionIV -> Stringified.ChampionIV
        | GameRank.ChampionV -> Stringified.ChampionV
        | GameRank.GrandChampion None -> Stringified.GrandChampion
        | GameRank.GrandChampion (Some elo) -> Stringified.grandChampionElo elo

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
        | Stringified.BronzeI -> Some GameRank.BronzeI
        | Stringified.BronzeII -> Some GameRank.BronzeII
        | Stringified.BronzeIII -> Some GameRank.BronzeIII
        | Stringified.BronzeIV -> Some GameRank.BronzeIV
        | Stringified.BronzeV -> Some GameRank.BronzeV
        | Stringified.SilverI -> Some GameRank.SilverI
        | Stringified.SilverII -> Some GameRank.SilverII
        | Stringified.SilverIII -> Some GameRank.SilverIII
        | Stringified.SilverIV -> Some GameRank.SilverIV
        | Stringified.SilverV -> Some GameRank.SilverV
        | Stringified.GoldI -> Some GameRank.GoldI
        | Stringified.GoldII -> Some GameRank.GoldII
        | Stringified.GoldIII -> Some GameRank.GoldIII
        | Stringified.GoldIV -> Some GameRank.GoldIV
        | Stringified.GoldV -> Some GameRank.GoldV
        | Stringified.PlatinumI -> Some GameRank.PlatinumI
        | Stringified.PlatinumII -> Some GameRank.PlatinumII
        | Stringified.PlatinumIII -> Some GameRank.PlatinumIII
        | Stringified.PlatinumIV -> Some GameRank.PlatinumIV
        | Stringified.PlatinumV -> Some GameRank.PlatinumV
        | Stringified.DiamondI -> Some GameRank.DiamondI
        | Stringified.DiamondII -> Some GameRank.DiamondII
        | Stringified.DiamondIII -> Some GameRank.DiamondIII
        | Stringified.DiamondIV -> Some GameRank.DiamondIV
        | Stringified.DiamondV -> Some GameRank.DiamondV
        | Stringified.ChampionI -> Some GameRank.ChampionI
        | Stringified.ChampionII -> Some GameRank.ChampionII
        | Stringified.ChampionIII -> Some GameRank.ChampionIII
        | Stringified.ChampionIV -> Some GameRank.ChampionIV
        | Stringified.ChampionV -> Some GameRank.ChampionV
        | Stringified.GrandChampion -> Some (GameRank.GrandChampion None)
        | GrandChampionElo rank -> Some rank
        | _ -> None

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
    module Stringified =
        let [<Literal>] Duel = "Duel"
        let [<Literal>] Doubles = "Doubles"
        let [<Literal>] Threes = "Threes"
        let [<Literal>] Hoops = "Hoops"
        let [<Literal>] Volleyball = "Volleyball"
        let [<Literal>] Squash = "Squash"

    let toString (v: GameMode) =
        match v with
        | GameMode.Duel -> Stringified.Duel
        | GameMode.Doubles -> Stringified.Doubles
        | GameMode.Threes -> Stringified.Threes
        | GameMode.Hoops -> Stringified.Hoops
        | GameMode.Volleyball -> Stringified.Volleyball
        | GameMode.Squash -> Stringified.Squash

    let fromString (str: string) =
        match str with
        | Stringified.Duel -> Some GameMode.Duel
        | Stringified.Doubles -> Some GameMode.Doubles
        | Stringified.Threes -> Some GameMode.Threes
        | Stringified.Hoops -> Some GameMode.Hoops
        | Stringified.Volleyball -> Some GameMode.Volleyball
        | Stringified.Squash -> Some GameMode.Squash
        | _ -> None

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
    module Stringified =
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

    let toString (v: GameServer) =
        match v with
        | GameServer.USE -> Stringified.USE
        | GameServer.USW -> Stringified.USW
        | GameServer.USC -> Stringified.USC
        | GameServer.EU -> Stringified.EU
        | GameServer.ASC -> Stringified.ASC
        | GameServer.ASM -> Stringified.ASM
        | GameServer.ME -> Stringified.ME
        | GameServer.OCE -> Stringified.OCE
        | GameServer.SAF -> Stringified.SAF
        | GameServer.SAM -> Stringified.SAM
        | GameServer.IND -> Stringified.IND

    let fromString (str: string) =
        match str with
        | Stringified.USE -> Some GameServer.USE
        | Stringified.USW -> Some GameServer.USW
        | Stringified.USC -> Some GameServer.USC
        | Stringified.EU -> Some GameServer.EU
        | Stringified.ASC -> Some GameServer.ASC
        | Stringified.ASM -> Some GameServer.ASM
        | Stringified.ME -> Some GameServer.ME
        | Stringified.OCE -> Some GameServer.OCE
        | Stringified.SAF -> Some GameServer.SAF
        | Stringified.SAM -> Some GameServer.SAM
        | Stringified.IND -> Some GameServer.IND
        | _ -> None
