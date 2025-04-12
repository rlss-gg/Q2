module Q2.Presentation.ProfileCommand

open FSharp.Discord.Commands
open FSharp.Discord.Types

module Options =
    let User =
        UserSubCommandOption.create "user" "The user whose profile you wish to view"
        |> UserSubCommandOption.setOptional

let Command =
    ChatInputCommand.create "profile" "View a player's profile"
    |> ChatInputCommand.setContexts [InteractionContextType.GUILD]
    |> ChatInputCommand.setIntegrations [ApplicationIntegrationType.GUILD_INSTALL]
    |> ChatInputCommand.addOption (SubCommandOption.User Options.User)
