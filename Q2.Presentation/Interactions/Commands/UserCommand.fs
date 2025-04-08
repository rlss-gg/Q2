module Q2.Presentation.UserCommand

open FSharp.Discord.Commands
open FSharp.Discord.Types

[<RequireQualifiedAccess>]
type Action =
    | InvalidArguments
    | VerifyGc of userId: string

module Metadata =
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
        |> ChatInputCommand.addSubCommandGroup Gc.Group

let (|Validate|_|) (interaction: Interaction) =
    match interaction with
    | ApplicationCommand Metadata.Command.Name { Options = Some (
        SubCommandGroup Metadata.Gc.Group.Name (
            SubCommand Metadata.Gc.Verify.SubCommand.Name (
                UserId.Required Metadata.Gc.Verify.Options.User.Name userId
            )
        )
    ) } ->
        Some (Action.VerifyGc userId)

    | ApplicationCommand Metadata.Command.Name _ ->
        Some Action.InvalidArguments

    | _ -> None
