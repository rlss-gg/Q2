namespace Q2.Application

open Microsoft.Azure.Functions.Worker
open Q2.Domain
open System
open System.Threading.Tasks

type QueueEntityId = | QueueEntityId of queueId: string * size: int

module QueueEntityId =
    let [<Literal>] SEPARATOR = '.'

    let create (queueId: string) (size: int) =
        QueueEntityId(queueId, size)

    let toString (queueId: string) (size: int) =
        queueId + (string SEPARATOR) + (string size)

    let tryParse (id: string) =
        id.Split SEPARATOR
        |> fun v -> Array.tryItem 0 v, (Array.tryItem 1 v |> Option.bind Int32.tryParse)
        |> function | Some queueId, Some size -> Some (QueueEntityId(queueId, size)) | _ -> None

type QueueEntity(time: ITime) =
    [<Function "QueueEntity">]
    member _.Dispatch ([<EntityTrigger>] dispatcher: TaskEntityDispatcher) =
        dispatcher.DispatchAsync(fun operation ->
            task {
                match QueueEntityId.tryParse operation.Context.Id.Key with
                | None -> return ()
                | Some (QueueEntityId (queueId, size)) ->
                    let state = operation.State.GetState(Queue.create queueId size)
                    let playerId = operation.GetInput<string>()
                    let currentTime = time.GetCurrentTime()

                    let updated =
                        match operation.Name with
                        | nameof Queue.addEntrant -> Queue.addEntrant playerId currentTime state
                        | nameof Queue.removeEntrant -> Queue.removeEntrant playerId state
                        | nameof Queue.setReady -> Queue.setReady playerId true state
                        | nameof Queue.clear -> Queue.clear state
                        | _ -> state

                    operation.State.SetState(updated)
            }
            |> ValueTask<obj>
        )
