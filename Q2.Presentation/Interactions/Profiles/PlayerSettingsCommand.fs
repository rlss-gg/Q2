module Q2.Presentation.PlayerSettingsCommand

open FSharp.Discord.Commands
open FSharp.Discord.Types

[<RequireQualifiedAccess>]
type Action =
    | InvalidArguments
    | ToggleNotifications of userId: string

module Metadata =
    module SubCommands =
        let Notifications =
            SubCommand.create "notifications" "Toggle channel pings when queues start"

    let Command =
        ChatInputCommand.create "settings" "Change or update settings on your account"
        |> ChatInputCommand.addSubCommand SubCommands.Notifications

let (|Validate|_|) (interaction: Interaction) =
    match interaction with
    | ApplicationCommand Metadata.Command.Name { Options = Some (
        SubCommand Metadata.SubCommands.Notifications.Name []
    ) } ->
        let userId =
            match interaction.Author with
            | InteractionAuthor.User u -> u.Id
            | InteractionAuthor.GuildMember m -> m.User |> Option.map _.Id |> Option.get

        Some (Action.ToggleNotifications userId)

    | ApplicationCommand Metadata.Command.Name _ ->
        Some Action.InvalidArguments

    | _ -> None
