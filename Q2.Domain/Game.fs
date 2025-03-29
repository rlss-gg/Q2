namespace Q2.Domain

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
    | GrandChampion of elo: int

/// Rocket League Sideswipe game modes
[<RequireQualifiedAccess>]
type GameMode =
    | Duel
    | Doubles
    | Threes
    | Hoops
    | Volleyball
    | Squash

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
