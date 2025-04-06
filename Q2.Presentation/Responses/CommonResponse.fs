module Q2.Presentation.CommonResponse

open FSharp.Discord.Rest
open FSharp.Discord.Types
open FSharp.Discord.Types.Serialization
open Thoth.Json.Net

let ping =
    let payload = { Type = InteractionCallbackType.PONG; Data = None }
    Encode.toString 0 (InteractionResponse.encoder payload)

let invalidArguments id token =
    let payload = CreateInteractionResponsePayload({
        Type = InteractionCallbackType.CHANNEL_MESSAGE_WITH_SOURCE
        Data = Some (InteractionCallbackData.MESSAGE {
            Tts = None
            Content = Some "Invalid arguments. Please check your input and try again."
            Embeds = None
            AllowedMentions = None
            Flags = Some [MessageFlag.EPHEMERAL]
            Components = None
            Attachments = None
            Poll = None
        })
    })

    CreateInteractionResponseRequest(id, token, payload)
    
let failed id token =
    let payload = CreateInteractionResponsePayload({
        Type = InteractionCallbackType.CHANNEL_MESSAGE_WITH_SOURCE
        Data = Some (InteractionCallbackData.MESSAGE {
            Tts = None
            Content = Some "Command failed. Please check your input and try again."
            Embeds = None
            AllowedMentions = None
            Flags = Some [MessageFlag.EPHEMERAL]
            Components = None
            Attachments = None
            Poll = None
        })
    })

    CreateInteractionResponseRequest(id, token, payload)

let notRegistered id token =
    let payload = CreateInteractionResponsePayload({
        Type = InteractionCallbackType.CHANNEL_MESSAGE_WITH_SOURCE
        Data = Some (InteractionCallbackData.MESSAGE {
            Tts = None
            Content = Some "You cannot use this command before registering."
            Embeds = None
            AllowedMentions = None
            Flags = Some [MessageFlag.EPHEMERAL]
            Components = None
            Attachments = None
            Poll = None
        })
    })

    CreateInteractionResponseRequest(id, token, payload)
