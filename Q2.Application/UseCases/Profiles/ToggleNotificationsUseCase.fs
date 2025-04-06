namespace Q2.Application

open Q2.Domain
open System.Threading.Tasks

type ToggleNotificationUseCase = {
    Id: string
}

[<RequireQualifiedAccess>]
type ToggleNotificationUseCaseError =
    | PlayerNotFound

module ToggleNotificationUseCase =
    let invoke player =
        player |> Player.setQueueNotifications (not player.Settings.QueueNotificationsEnabled)

    let run (env: #IPersistence) (req: ToggleNotificationUseCase) = task {
        match! env.Players.Get req.Id with
        | None ->
            return Error ToggleNotificationUseCaseError.PlayerNotFound

        | Some player ->
            let res = invoke player
            return! env.Players.Set res |> Task.map Ok
    }
