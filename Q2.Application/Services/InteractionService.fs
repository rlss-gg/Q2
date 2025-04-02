module Q2.Application.InteractionService

open FSharp.Discord.Commands
open FSharp.Discord.Types
open Q2.Domain

let handle env (interaction: Interaction) = task {
    match interaction with
    | ApplicationCommandAutocomplete "register" { Options = Some (
        String.Autocomplete "server" server
    ) } ->
        return () // TODO: Implement

    | ApplicationCommandAutocomplete "register" { Options = Some (
        String.Autocomplete "rank" rank
    ) } ->
        return () // TODO: Implement

    | ApplicationCommandAutocomplete "register" { Options = Some (
        String.Autocomplete "mode" mode
    ) } ->
        return () // TODO: Implement

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

        let server = GameServer.fromString server
        let rank = GameRank.fromString rank
        let mode = GameMode.fromString mode

        // TODO: These can probably be changed to choices which can then match perfectly (how to handle rank then?)
        // TODO: If going the choices route, can probably make special options active patterns for extracting to enum
        // TODO: Do we really want mode and specific region? How could this be improved?

        match server, rank, mode with
        | Some server, Some rank, Some mode ->
            let! res = RegisterPlayerUseCase.run env {
                Id = userId
                Username = username
                PrimaryServer = server
                Rank = rank
                RankMode = mode
            }

            return ()

            // TODO: This should probably be the individual command's responsibility which then returns unit

        | _ ->
            return ()

        // TODO: Rather than a big match, maybe each command can be separately defined and implemented, and this just
        //       picks which to run. Maybe it should also handle the response?

    | _ ->
        return ()

    // TODO: Should this be enqueued instead so the function can return 202 immediately?
}
