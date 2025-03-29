module Q2.Infrastructure.Persistence.Cosmos

open Microsoft.Azure.Cosmos
open Q2.Domain
open System.Threading.Tasks

let [<Literal>] DATABASE_NAME = "q2"

module Player =
    let [<Literal>] CONTAINER_NAME = "players"

    let container (cosmosClient: CosmosClient) =
        cosmosClient.GetContainer(DATABASE_NAME, CONTAINER_NAME)

    let get (playerId: string) (cosmosClient: CosmosClient) =
        try
            container cosmosClient
            |> _.ReadItemAsync<Player>(playerId, PartitionKey playerId)
            |> Task.map (fun res -> res.Resource |> Some)

        with | _ -> Task.lift None

    let set (player: Player) (cosmosClient: CosmosClient) =
        container cosmosClient
        |> _.UpsertItemAsync<Player>(player, PartitionKey player.Id)
        |> Task.map (fun res -> res.Resource)
        
module Rank =
    let [<Literal>] CONTAINER_NAME = "ranks"

    let container (cosmosClient: CosmosClient) =
        cosmosClient.GetContainer(DATABASE_NAME, CONTAINER_NAME)

    let get (rankId: string) (cosmosClient: CosmosClient) =
        try
            container cosmosClient
            |> _.ReadItemAsync<Rank>(rankId, PartitionKey rankId)
            |> Task.map (fun res -> res.Resource |> Some)

        with | _ -> Task.lift None

    let set (rank: Rank) (cosmosClient: CosmosClient) =
        container cosmosClient
        |> _.UpsertItemAsync<Rank>(rank, PartitionKey rank.Id)
        |> Task.map (fun res -> res.Resource)
        
module Guild =
    let [<Literal>] CONTAINER_NAME = "guilds"

    let container (cosmosClient: CosmosClient) =
        cosmosClient.GetContainer(DATABASE_NAME, CONTAINER_NAME)

    let get (guildId: string) (cosmosClient: CosmosClient) =
        try
            container cosmosClient
            |> _.ReadItemAsync<Guild>(guildId, PartitionKey guildId)
            |> Task.map (fun res -> res.Resource |> Some)

        with | _ -> Task.lift None

    let set (guild: Guild) (cosmosClient: CosmosClient) =
        container cosmosClient
        |> _.UpsertItemAsync<Guild>(guild, PartitionKey guild.Id)
        |> Task.map (fun res -> res.Resource)
        
module Queue =
    let [<Literal>] CONTAINER_NAME = "queues"

    let container (cosmosClient: CosmosClient) =
        cosmosClient.GetContainer(DATABASE_NAME, CONTAINER_NAME)

    let get (queueId: string) (cosmosClient: CosmosClient) =
        try
            container cosmosClient
            |> _.ReadItemAsync<Queue>(queueId, PartitionKey queueId)
            |> Task.map (fun res -> res.Resource |> Some)

        with | _ -> Task.lift None

    let set (queue: Queue) (cosmosClient: CosmosClient) =
        container cosmosClient
        |> _.UpsertItemAsync<Queue>(queue, PartitionKey queue.Id)
        |> Task.map (fun res -> res.Resource)
        
module Match =
    let [<Literal>] CONTAINER_NAME = "matches"

    let container (cosmosClient: CosmosClient) =
        cosmosClient.GetContainer(DATABASE_NAME, CONTAINER_NAME)

    let get (matchId: string) (cosmosClient: CosmosClient) =
        try
            container cosmosClient
            |> _.ReadItemAsync<Match>(matchId, PartitionKey matchId)
            |> Task.map (fun res -> res.Resource |> Some)

        with | _ -> Task.lift None

    let set (match': Match) (cosmosClient: CosmosClient) =
        container cosmosClient
        |> _.UpsertItemAsync<Match>(match', PartitionKey match'.Id)
        |> Task.map (fun res -> res.Resource)
