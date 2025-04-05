module Q2.Presentation.RegisterPlayerResponse

open FSharp.Discord.Rest
open FSharp.Discord.Types

let rankAutocomplete id token choices =
    let payload = CreateInteractionResponsePayload({
        Type = InteractionCallbackType.APPLICATION_COMMAND_AUTOCOMPLETE_RESULT
        Data = Some (InteractionCallbackData.AUTOCOMPLETE {
            Choices =
                choices
                |> List.map (fun v -> {
                    Name = v
                    NameLocalizations = None
                    Value = ApplicationCommandOptionChoiceValue.STRING v
                })
        })
    })

    CreateInteractionResponseRequest(id, token, payload)

let playerAlreadyRegistered id token =
    let payload = CreateInteractionResponsePayload({
        Type = InteractionCallbackType.CHANNEL_MESSAGE_WITH_SOURCE
        Data = Some (InteractionCallbackData.MESSAGE {
            Tts = None
            Content = Some "You are already registered."
            Embeds = None
            AllowedMentions = None
            Flags = Some [MessageFlag.EPHEMERAL]
            Components = None
            Attachments = None
            Poll = None
        })
    })

    CreateInteractionResponseRequest(id, token, payload)

let runCommand id token unit =
    // TODO: Implement

    let payload = CreateInteractionResponsePayload({
        Type = InteractionCallbackType.CHANNEL_MESSAGE_WITH_SOURCE
        Data = Some (InteractionCallbackData.MESSAGE {
            Tts = None
            Content = Some "Hello world!"
            Embeds = None
            AllowedMentions = None
            Flags = None
            Components = None
            Attachments = None
            Poll = None
        })
    })

    CreateInteractionResponseRequest(id, token, payload)
