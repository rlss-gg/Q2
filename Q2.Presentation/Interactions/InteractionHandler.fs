namespace Q2.Presentation

open FSharp.Discord.Commands
open FSharp.Discord.Rest
open FSharp.Discord.Types
open FSharp.Discord.Webhook
open Q2.Application
open Q2.Domain
open System.Threading.Tasks

type InteractionHandlerResponse =
    | Success
    | Ping
    | Failure

module InteractionHandler =
    let handle (env: IEnv) (event: InteractionCreateEvent) (interaction: Interaction) = task {
        let client = env.BotClient env.DiscordBotToken

        let authorId =
            match interaction.Author with
            | InteractionAuthor.User u -> u.Id
            | InteractionAuthor.GuildMember m -> m.User |> Option.map _.Id |> Option.get

        let respond (response: CreateInteractionResponseRequest) = task {
            do! Rest.createInteractionResponse response client :> Task
            return InteractionHandlerResponse.Success
        }

        match event with
        | InteractionCreateEvent.PING ->
            return InteractionHandlerResponse.Ping

        | InteractionCreateEvent.APPLICATION_COMMAND_AUTOCOMPLETE ({
            Name = Equals RegisterCommand.Command.Name
            Options = Some (String.Autocomplete RegisterCommand.Options.Rank.Name query)
        })  ->
            let choices = RankAutocompleteUseCase.run { Query = query }
            let response = RegisterPlayerResponse.rankAutocomplete interaction.Id interaction.Token choices
            return! respond response

        | InteractionCreateEvent.APPLICATION_COMMAND ({
            Name = Equals RegisterCommand.Command.Name
            Options = Some (
                String.Required RegisterCommand.Options.Username.Name username &
                String.Required RegisterCommand.Options.Region.Name (Region.Region region) &
                String.Required RegisterCommand.Options.Rank.Name (GameRank.GameRank rank)
            )
        }) ->
            let data: RegisterPlayerUseCase = { Id = authorId; Username = username; Region = region; Rank = rank }

            match! RegisterPlayerUseCase.run env data with
            | Error RegisterPlayerUseCaseError.PlayerAlreadyRegistered ->
                let response = CommonResponse.failed interaction.Id interaction.Token
                return! respond response

            | Ok player ->
                let response = RegisterPlayerResponse.runCommand interaction.Id interaction.Token player
                return! respond response
                
        | InteractionCreateEvent.APPLICATION_COMMAND ({
            Name = Equals SettingsCommand.Command.Name
            Options = Some (SubCommand SettingsCommand.SubCommands.Notifications.Name [])
        }) ->
            let data: ToggleNotificationUseCase = { Id = authorId }

            match! ToggleNotificationUseCase.run env data with
            | Error ToggleNotificationUseCaseError.PlayerNotFound ->
                let response = CommonResponse.notRegistered interaction.Id interaction.Token
                return! respond response

            | Ok player ->
                let response = PlayerSettingsResponse.notifications interaction.Id interaction.Token player.Settings.QueueNotificationsEnabled
                return! respond response

        | InteractionCreateEvent.APPLICATION_COMMAND ({
            Name = Equals ProfileCommand.Command.Name
            Options = Some (UserId.Optional ProfileCommand.Options.User.Name userId)
        }) ->
            let data: GetPlayerUseCase = { Id = Option.defaultValue authorId userId }

            match! GetPlayerUseCase.run env data with
            | Error GetPlayerUseCaseError.PlayerNotFound ->
                let response = CommonResponse.notRegistered interaction.Id interaction.Token
                return! respond response

            | Ok player ->
                let response = PlayerProfileResponse.profile interaction.Id interaction.Token player
                return! respond response

        | InteractionCreateEvent.APPLICATION_COMMAND ({
            Name = Equals UserCommand.Command.Name
            Options = Some (
                SubCommandGroup UserCommand.Gc.Group.Name (
                    SubCommand UserCommand.Gc.Verify.SubCommand.Name (
                        UserId.Required UserCommand.Gc.Verify.Options.User.Name userId
                    )
                )
            )
        }) ->
            let data: VerifyPlayerGcUseCase = { Id = userId }

            match! VerifyPlayerGcUseCase.run env data with
            | Error VerifyPlayerGcUseCaseError.PlayerNotFound ->
                let response = CommonResponse.notRegistered interaction.Id interaction.Token
                return! respond response

            | Error VerifyPlayerGcUseCaseError.PrimaryRankNotGc ->
                let response = UserManagementResponse.playerNotGc interaction.Id interaction.Token
                return! respond response

            | Ok player ->
                let response = UserManagementResponse.verifyGc interaction.Id interaction.Token player
                return! respond response
            
        | _ ->
            return InteractionHandlerResponse.Failure
    }
