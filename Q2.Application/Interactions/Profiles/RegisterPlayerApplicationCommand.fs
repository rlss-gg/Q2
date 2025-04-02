module Q2.Application.RegisterPlayerApplicationCommand

open FSharp.Discord.Commands
open FSharp.Discord.Types
open Q2.Domain

[<RequireQualifiedAccess>]
type Action =
    | ServerAutocomplete of string
    | RankAutocomplete   of string
    | ModeAutocomplete   of string
    | RunCommand         of RegisterPlayerUseCase

// TODO: Should these contain use case payloads?

let (|Validate|_|) (interaction: Interaction) =
    // TODO: Do we really want mode and specific region? How could this be improved?
    // TODO: Preferably autocomplete could just be replaced with choices

    match interaction with
    | ApplicationCommandAutocomplete "register" { Options = Some (
        String.Autocomplete "server" server
    ) } ->
        Some (Action.ServerAutocomplete server)

    | ApplicationCommandAutocomplete "register" { Options = Some (
        String.Autocomplete "rank" rank
    ) } ->
        Some (Action.RankAutocomplete rank)

    | ApplicationCommandAutocomplete "register" { Options = Some (
        String.Autocomplete "mode" mode
    ) } ->
        Some (Action.ModeAutocomplete mode)

    | ApplicationCommand "register" { Options = Some (
        String.Required "username" username &
        String.Required "server" server &
        String.Required "rank" rank &
        String.Required "mode" mode
    ) } ->
        let userId =
            match interaction.Author with
            | InteractionAuthor.User u -> u.Id
            | InteractionAuthor.GuildMember m -> m.User |> Option.map _.Id |> Option.get

        Some (Action.RunCommand {
            Id = userId
            Username = username
            PrimaryServer = GameServer.fromString server |> Option.get
            Rank = GameRank.fromString rank |> Option.get
            RankMode = GameMode.fromString mode |> Option.get
        })

    | _ -> None

// TODO: Function to run the given actions (?)
