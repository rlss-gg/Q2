module Q2.Application.InteractionService

// TODO: Figure out a more appropriate name for this module

let handle env interaction = task {
    match interaction with
    | RegisterPlayerApplicationCommand.Validate action -> return ()
    | _ -> return ()

    // TODO: Figure out how the actions should then be called to run
    // TODO: Should this be enqueued instead so the function can return 202 immediately?
}
