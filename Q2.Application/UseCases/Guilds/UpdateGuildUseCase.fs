module Q2.Application.UpdateGuildUseCase

open Q2.Domain
open System.Threading.Tasks

type UpdateGuildUseCaseRequest = {
    Id: string
    HelperRoleId: string option option
    ModeratorRoleId: string option option
    AdminRoleId: string option option
    RankRoles: Map<string, string> option
    ReportingChannelId: string option option
    RankChannels: Map<string, string> option
}

[<RequireQualifiedAccess>]
type UpdateGuildUseCaseError =
    | GuildDoesNotExist

let invoke helperRoleId moderatorRoleId adminRoleId rankRoles reportingChannelId rankChannels guild =
    guild
    |> Option.foldBack (fun roleId guild -> Guild.setHelperRole roleId guild) helperRoleId
    |> Option.foldBack (fun roleId guild -> Guild.setModeratorRole roleId guild) moderatorRoleId
    |> Option.foldBack (fun roleId guild -> Guild.setAdministratorRole roleId guild) adminRoleId
    |> Option.foldBack (fun roles guild -> Guild.setRankRoles roles guild) rankRoles
    |> Option.foldBack (fun channelId guild -> Guild.setReportingChannel channelId guild) reportingChannelId
    |> Option.foldBack (fun channels guild -> Guild.setRankChannels channels guild) rankChannels

let run (env: #IPersistence) (req: UpdateGuildUseCaseRequest) = task {
    match! env.Guilds.Get req.Id with
    | None ->
        return Error UpdateGuildUseCaseError.GuildDoesNotExist

    | Some guild ->
        let res = invoke req.HelperRoleId req.ModeratorRoleId req.AdminRoleId req.RankRoles req.ReportingChannelId req.RankChannels guild
        return! env.Guilds.Set res |> Task.map Ok
}
