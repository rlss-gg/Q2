module Q2.Presentation.RegisterCommand

open FSharp.Discord.Commands
open FSharp.Discord.Types
open Q2.Domain

module Options =
    let Username =
        StringSubCommandOption.create "username" "Your Epic account username"
        |> StringSubCommandOption.setRequired

    let Region =
        StringSubCommandOption.create "region" "Your region"
        |> StringSubCommandOption.setRequired
        |> StringSubCommandOption.setChoices [
            StringSubCommandOptionChoice.create "North America" Region.Serialization.NA
            StringSubCommandOptionChoice.create "Europe" Region.Serialization.EU
            StringSubCommandOptionChoice.create "Asia" Region.Serialization.ASIA
            StringSubCommandOptionChoice.create "Middle East" Region.Serialization.MENA
            StringSubCommandOptionChoice.create "Oceania" Region.Serialization.OCE
            StringSubCommandOptionChoice.create "South Africa" Region.Serialization.SAF
            StringSubCommandOptionChoice.create "South America" Region.Serialization.SAM
        ]

    let Rank =
        StringSubCommandOption.create "rank" "Your current in-game rank"
        |> StringSubCommandOption.setRequired
        |> StringSubCommandOption.setAutocomplete

let Command =
    ChatInputCommand.create "register" "Register your account information for Q2"
    |> ChatInputCommand.setContexts [InteractionContextType.GUILD]
    |> ChatInputCommand.setIntegrations [ApplicationIntegrationType.GUILD_INSTALL]
    |> ChatInputCommand.addOption (SubCommandOption.String Options.Username)
    |> ChatInputCommand.addOption (SubCommandOption.String Options.Region)
    |> ChatInputCommand.addOption (SubCommandOption.String Options.Rank)
