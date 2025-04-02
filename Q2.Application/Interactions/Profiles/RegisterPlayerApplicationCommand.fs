module Q2.Application.RegisterPlayerApplicationCommand

open FSharp.Discord.Commands
open FSharp.Discord.Types
open Q2.Domain

[<RequireQualifiedAccess>]
type Action =
    | InvalidArguments
    | RankAutocomplete   of query: string
    | RunCommand         of userId: string * username: string * region: Region * rank: GameRank

let (|Validate|_|) (interaction: Interaction) =
    match interaction with
    | ApplicationCommandAutocomplete "register" { Options = Some (
        String.Autocomplete "rank" rank
    ) } ->
        Some (Action.RankAutocomplete rank)

    | ApplicationCommand "register" { Options = Some (
        String.Required "username" username &
        String.Required "region" (Region.Region region) &
        String.Required "rank" (GameRank.GameRank rank)
    ) } ->
        let userId =
            match interaction.Author with
            | InteractionAuthor.User u -> u.Id
            | InteractionAuthor.GuildMember m -> m.User |> Option.map _.Id |> Option.get

        Some (Action.RunCommand(userId, username, region, rank))

    | ApplicationCommand "register" _ ->
        Some Action.InvalidArguments

    | _ ->
        None

// TODO: Function to run the given actions (?)
// TODO: How to prevent repeating strings for names of commands and options?
