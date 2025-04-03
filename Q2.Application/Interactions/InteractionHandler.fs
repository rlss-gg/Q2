module Q2.Application.InteractionHandler

open FSharp.Discord.Rest
open FSharp.Discord.Types
open System.Threading.Tasks

let handle (env: #IHttp & #ISecrets) interaction = task {
    let client = env.BotClient env.DiscordBotToken

    match interaction with
    | RegisterPlayerApplicationCommand.Validate action ->
        match action with
        | RegisterPlayerApplicationCommand.Action.InvalidArguments ->
            let req = CreateInteractionResponseRequest(
                interaction.Id,
                interaction.Token,
                CreateInteractionResponsePayload({
                    Type = InteractionCallbackType.CHANNEL_MESSAGE_WITH_SOURCE
                    Data = Some (InteractionCallbackData.MESSAGE {
                        Tts = None
                        Content = Some "Invalid arguments. Please check your input and try again."
                        Embeds = None
                        AllowedMentions = None
                        Flags = None
                        Components = None
                        Attachments = None
                        Poll = None
                    })
                }))

            do! Rest.createInteractionResponse req client :> Task

            // TODO: Create "response models" for responses so streamline this

        | RegisterPlayerApplicationCommand.Action.RankAutocomplete query ->
            let req = CreateInteractionResponseRequest(
                interaction.Id,
                interaction.Token,
                CreateInteractionResponsePayload({
                    Type = InteractionCallbackType.APPLICATION_COMMAND_AUTOCOMPLETE_RESULT
                    Data = Some (InteractionCallbackData.AUTOCOMPLETE {
                        Choices = []
                    })
                }))

            do! Rest.createInteractionResponse req client :> Task

        | RegisterPlayerApplicationCommand.Action.RunCommand (userId, username, region, rank) ->
            let req = CreateInteractionResponseRequest(
                interaction.Id,
                interaction.Token,
                CreateInteractionResponsePayload({
                    Type = InteractionCallbackType.CHANNEL_MESSAGE_WITH_SOURCE
                    Data = Some (InteractionCallbackData.MESSAGE {
                        Tts = None
                        Content = Some "Hello world"
                        Embeds = None
                        AllowedMentions = None
                        Flags = None
                        Components = None
                        Attachments = None
                        Poll = None
                    })
                }))

            do! Rest.createInteractionResponse req client :> Task

            // TODO: Call use case and map result to response (and replace temporary response above)

        // TODO: Move this logic into a more appropriate place (probably command in the command file)

    | _ -> return ()

    // TODO: Should this be enqueued instead so the function can return 202 immediately?
}
