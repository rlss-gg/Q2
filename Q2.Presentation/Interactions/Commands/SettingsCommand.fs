module Q2.Presentation.SettingsCommand

open FSharp.Discord.Commands
open FSharp.Discord.Types

module SubCommands =
    let Notifications =
        SubCommand.create "notifications" "Toggle channel pings when queues start"

let Command =
    ChatInputCommand.create "settings" "Change or update settings on your account"
    |> ChatInputCommand.setContexts [InteractionContextType.GUILD]
    |> ChatInputCommand.setIntegrations [ApplicationIntegrationType.GUILD_INSTALL]
    |> ChatInputCommand.addSubCommand SubCommands.Notifications
