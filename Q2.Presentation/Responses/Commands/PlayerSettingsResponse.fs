module Q2.Presentation.PlayerSettingsResponse

open FSharp.Discord.Rest
open FSharp.Discord.Types

let notifications id token (enabled: bool) =
    let state =
        match enabled with
        | true -> "enabled"
        | false -> "disabled"

    let payload = CreateInteractionResponsePayload({
        Type = InteractionCallbackType.CHANNEL_MESSAGE_WITH_SOURCE
        Data = Some (InteractionCallbackData.MESSAGE {
            Tts = None
            Content = Some $"Notifications successfully **{state}**."
            Embeds = None
            AllowedMentions = None
            Flags = Some [MessageFlag.EPHEMERAL]
            Components = None
            Attachments = None
            Poll = None
        })
    })

    CreateInteractionResponseRequest(id, token, payload)
