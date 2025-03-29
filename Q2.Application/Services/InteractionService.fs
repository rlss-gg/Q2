module Q2.Application.InteractionService

open FSharp.Discord.Types

let handle (interaction: Interaction) = task {
    // TODO: Match the interaction to command here and call use cases
    // TODO: Partially apply the use cases for testability
    // TODO: Should this be enqueued so the function can reutrn 202 immediately?

    return ()
}
