namespace Q2.Domain

type GuildRoles = {
    HelperRoleId: string option
    ModeratorRoleId: string option
    AdministratorRoleId: string option
}

type GuildChannels = {
    ReportingChannelId: string option
    Ranks: Map<string, string>
}

type Guild = {
    Id: string
    Roles: GuildRoles
    Channels: GuildChannels
    Progressions: RankProgression list
}
