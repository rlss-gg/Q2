module Q2.Application.InteractionHandler

let handle env interaction = task {
    match interaction with
    | RegisterPlayerApplicationCommand.Validate action ->
        match action with
        | RegisterPlayerApplicationCommand.Action.InvalidArguments ->
            return ()

        | RegisterPlayerApplicationCommand.Action.RankAutocomplete query ->
            return ()

        | RegisterPlayerApplicationCommand.Action.RunCommand (userId, username, region, rank) ->
            return ()

    | _ -> return ()

    // TODO: Should this be enqueued instead so the function can return 202 immediately?
}
