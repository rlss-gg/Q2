module Q2.Presentation.RegisterCommand

open FSharp.Discord.Commands
open FSharp.Discord.Types
open Q2.Domain

[<RequireQualifiedAccess>]
type Action =
    | InvalidArguments
    | RankAutocomplete of query: string
    | RunCommand       of userId: string * username: string * region: Region * rank: GameRank

module Metadata =
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
        |> ChatInputCommand.addOption (SubCommandOption.String Options.Username)
        |> ChatInputCommand.addOption (SubCommandOption.String Options.Region)
        |> ChatInputCommand.addOption (SubCommandOption.String Options.Rank)

let (|Validate|_|) (interaction: Interaction) =
    match interaction with
    | ApplicationCommandAutocomplete Metadata.Command.Name { Options = Some (
        String.Autocomplete Metadata.Options.Rank.Name rank
    ) } ->
        Some (Action.RankAutocomplete rank)

    | ApplicationCommand Metadata.Command.Name { Options = Some (
        String.Required Metadata.Options.Username.Name username &
        String.Required Metadata.Options.Region.Name (Region.Region region) &
        String.Required Metadata.Options.Rank.Name (GameRank.GameRank rank)
    ) } ->
        let userId =
            match interaction.Author with
            | InteractionAuthor.User u -> u.Id
            | InteractionAuthor.GuildMember m -> m.User |> Option.map _.Id |> Option.get

        Some (Action.RunCommand(userId, username, region, rank))

    | ApplicationCommand Metadata.Command.Name _ ->
        Some Action.InvalidArguments

    | _ -> None
