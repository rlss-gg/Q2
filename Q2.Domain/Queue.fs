namespace Q2.Domain

open System

type Queue = {
    Id: string
    Size: int
    Entrants: Map<string, DateTime>
    Ready: string list
}

module Queue =
    let create id size =
        {
            Id = id
            Size = size
            Entrants = Map.empty
            Ready = []
        }

    let addEntrant playerId currentTime queue =
        { queue with Entrants = queue.Entrants |> Map.add playerId currentTime }

    let removeEntrant playerId queue =
        { queue with Entrants = queue.Entrants |> Map.remove playerId }
        
    let getFilledTime queue =
        let folder acc _ cur =
            if (Option.defaultValue DateTime.MinValue acc) < cur
            then Some cur
            else acc

        match Map.count queue.Entrants = queue.Size with
        | true -> queue.Entrants |> Map.fold folder None
        | false -> None

    let setReady playerId ready queue =
        match ready with
        | true -> { queue with Ready = queue.Ready @ [playerId] |> List.distinct }
        | false -> { queue with Ready = queue.Ready |> List.filter ((<>) playerId) }

    let isReady queue =
        List.length queue.Ready = Map.count queue.Entrants
    
    let clear queue =
        create queue.Id queue.Size
