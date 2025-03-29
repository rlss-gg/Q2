namespace Q2.Domain

type GuildRoles = {
    Ranks: Map<string, string>
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
}
