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

module Guild =
    let create id =
        {
            Id = id
            Roles = {
                Ranks = Map.empty
                HelperRoleId = None
                ModeratorRoleId = None
                AdministratorRoleId = None
            }
            Channels = {
                ReportingChannelId = None
                Ranks = Map.empty
            }
        }

    let setRankRoles ranks guild =
        let channels = { guild.Channels with Ranks = ranks }
        { guild with Channels = channels }

    let addRankRole rankId roleId guild =
        let ranks = guild.Roles.Ranks |> Map.add rankId roleId
        setRankRoles ranks guild

    let removeRankRole rankId guild =
        let ranks = guild.Roles.Ranks |> Map.remove rankId
        setRankRoles ranks guild

    let setHelperRole roleId guild =
        let roles = { guild.Roles with HelperRoleId = roleId }
        { guild with Roles = roles }

    let setModeratorRole roleId guild =
        let roles = { guild.Roles with ModeratorRoleId = roleId }
        { guild with Roles = roles }

    let setAdministratorRole roleId guild =
        let roles = { guild.Roles with AdministratorRoleId = roleId }
        { guild with Roles = roles }

    let setReportingChannel channelId guild =
        let channels = { guild.Channels with ReportingChannelId = channelId }
        { guild with Channels = channels }

    let setRankChannels channels guild =
        let channels = { guild.Channels with Ranks = channels }
        { guild with Channels = channels }

    let addRankChannel rankId channelId guild =
        let channels = guild.Channels.Ranks |> Map.add rankId channelId
        setRankChannels channels guild

    let removeRankChannel rankId guild =
        let channels = guild.Channels.Ranks |> Map.remove rankId
        setRankChannels channels guild
