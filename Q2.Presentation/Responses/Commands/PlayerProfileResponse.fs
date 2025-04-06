module Q2.Presentation.PlayerProfileResponse

open FSharp.Discord.Rest
open FSharp.Discord.Types
open Q2.Domain

let profile id token (player: Player) =
    // TODO: Implement

    let payload = CreateInteractionResponsePayload({
        Type = InteractionCallbackType.CHANNEL_MESSAGE_WITH_SOURCE
        Data = Some (InteractionCallbackData.MESSAGE {
            Tts = None
            Content = Some $"Hello world!"
            Embeds = None
            AllowedMentions = None
            Flags = None
            Components = None
            Attachments = None
            Poll = None
        })
    })

    CreateInteractionResponseRequest(id, token, payload)
