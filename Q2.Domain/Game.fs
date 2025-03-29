namespace Q2.Domain

/// Rocket League Sideswipe competitive ranks
[<RequireQualifiedAccess>]
type GameRank =
    | Bronze
    | Silver
    | Gold
    | Platinum
    | Diamond
    | Champion
    | GrandChampion

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
