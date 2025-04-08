module Q2.Presentation.ProfileCommand

open FSharp.Discord.Commands
open FSharp.Discord.Types

[<RequireQualifiedAccess>]
type Action =
    | InvalidArguments
    | ViewProfile of userId: string

module Metadata =
    module Options =
        let User =
            UserSubCommandOption.create "user" "The user whose profile you wish to view"
            |> UserSubCommandOption.setOptional

    let Command =
        ChatInputCommand.create "profile" "Change or update settings on your account"
        |> ChatInputCommand.setContexts [InteractionContextType.GUILD]
        |> ChatInputCommand.setIntegrations [ApplicationIntegrationType.GUILD_INSTALL]
        |> ChatInputCommand.addOption (SubCommandOption.User Options.User)

let (|Validate|_|) (interaction: Interaction) =
    match interaction with
    | ApplicationCommand Metadata.Command.Name { Options = Some (
        UserId.Optional Metadata.Options.User.Name providedId
    ) } ->
        let authorId =
            match interaction.Author with
            | InteractionAuthor.User u -> u.Id
            | InteractionAuthor.GuildMember m -> m.User |> Option.map _.Id |> Option.get

        let userId = Option.defaultValue authorId providedId

        Some (Action.ViewProfile userId)

    | ApplicationCommand Metadata.Command.Name _ ->
        Some Action.InvalidArguments

    | _ -> None
