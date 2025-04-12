module Q2.Presentation.UserCommand

open FSharp.Discord.Commands
open FSharp.Discord.Types

module Gc =
    module Verify =
        module Options =
            let User =
                UserSubCommandOption.create "user" "The user to verify GC"
                |> UserSubCommandOption.setRequired

        let SubCommand =
            SubCommand.create "verify" "Verify a user as a GC"
            |> SubCommand.addOption (SubCommandOption.User Options.User)

    let Group =
        SubCommandGroup.create "gc" "Manage the GC verification status for the user"
        |> SubCommandGroup.addSubCommand Verify.SubCommand

let Command =
    ChatInputCommand.create "user" "Manage users"
    |> ChatInputCommand.setAdministratorOnly
    |> ChatInputCommand.setContexts [InteractionContextType.GUILD]
    |> ChatInputCommand.setIntegrations [ApplicationIntegrationType.GUILD_INSTALL]
    |> ChatInputCommand.addSubCommandGroup Gc.Group
