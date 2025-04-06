module Q2.Presentation.UserManagementResponse

open FSharp.Discord.Rest
open FSharp.Discord.Types
open Q2.Domain

let playerNotGc id token =
    let payload = CreateInteractionResponsePayload({
        Type = InteractionCallbackType.CHANNEL_MESSAGE_WITH_SOURCE
        Data = Some (InteractionCallbackData.MESSAGE {
            Tts = None
            Content = Some $"The given player has not indicated they are a GC."
            Embeds = None
            AllowedMentions = None
            Flags = Some [MessageFlag.EPHEMERAL]
            Components = None
            Attachments = None
            Poll = None
        })
    })

    CreateInteractionResponseRequest(id, token, payload)


let verifyGc id token (player: Player) =
    let payload = CreateInteractionResponsePayload({
        Type = InteractionCallbackType.CHANNEL_MESSAGE_WITH_SOURCE
        Data = Some (InteractionCallbackData.MESSAGE {
            Tts = None
            Content = Some $"Successfully verified {player.Username} as a GC."
            Embeds = None
            AllowedMentions = None
            Flags = None
            Components = None
            Attachments = None
            Poll = None
        })
    })

    CreateInteractionResponseRequest(id, token, payload)
