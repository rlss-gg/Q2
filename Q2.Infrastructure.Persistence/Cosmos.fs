module Q2.Infrastructure.Persistence.Cosmos

open Microsoft.Azure.Cosmos
open Newtonsoft.Json
open Newtonsoft.Json.Serialization
open Q2.Domain
open System.IO
open System.Text

let [<Literal>] DATABASE_NAME = "q2"

module internal Container =
    let options = JsonSerializerSettings(ContractResolver = CamelCasePropertyNamesContractResolver())

    let read<'T> (id: string) (partitionKey: PartitionKey) (container: Container) = task {
        try
            let! res = container.ReadItemStreamAsync(id, partitionKey)

            let reader = new StreamReader(res.Content)
            let! result = reader.ReadToEndAsync()
            return Some (JsonConvert.DeserializeObject<'T>(result, options))

        with | _ -> return None
    }

    let upsert (item: 'T) (partitionKey: PartitionKey) (container: Container) = task {
        let json = JsonConvert.SerializeObject(item, options)
        let bytes = Encoding.UTF8.GetBytes json
        use stream = new MemoryStream(bytes)

        let! res = container.UpsertItemStreamAsync(stream, partitionKey)

        let reader = new StreamReader(res.Content)
        let! result = reader.ReadToEndAsync()
        return JsonConvert.DeserializeObject<'T>(result, options)

        // TODO: Handle errors gracefully
    }

module Player =
    let [<Literal>] CONTAINER_NAME = "players"

    let container (cosmosClient: CosmosClient) =
        cosmosClient.GetContainer(DATABASE_NAME, CONTAINER_NAME)

    let get (playerId: string) (cosmosClient: CosmosClient) =
        Container.read<Player> playerId (PartitionKey playerId) (container cosmosClient)

    let set (player: Player) (cosmosClient: CosmosClient) =
        Container.upsert player (PartitionKey player.Id) (container cosmosClient)
        
module Rank =
    let [<Literal>] CONTAINER_NAME = "ranks"

    let container (cosmosClient: CosmosClient) =
        cosmosClient.GetContainer(DATABASE_NAME, CONTAINER_NAME)

    let get (rankId: string) (cosmosClient: CosmosClient) =
        Container.read<Rank> rankId (PartitionKey rankId) (container cosmosClient)

    let set (rank: Rank) (cosmosClient: CosmosClient) =
        Container.upsert rank (PartitionKey rank.Id) (container cosmosClient)
        
module Guild =
    let [<Literal>] CONTAINER_NAME = "guilds"

    let container (cosmosClient: CosmosClient) =
        cosmosClient.GetContainer(DATABASE_NAME, CONTAINER_NAME)

    let get (guildId: string) (cosmosClient: CosmosClient) =
        Container.read<Guild> guildId (PartitionKey guildId) (container cosmosClient)

    let set (guild: Guild) (cosmosClient: CosmosClient) =
        Container.upsert guild (PartitionKey guild.Id) (container cosmosClient)
        
module Queue =
    let [<Literal>] CONTAINER_NAME = "queues"

    let container (cosmosClient: CosmosClient) =
        cosmosClient.GetContainer(DATABASE_NAME, CONTAINER_NAME)

    let get (queueId: string) (cosmosClient: CosmosClient) =
        Container.read<Queue> queueId (PartitionKey queueId) (container cosmosClient)

    let set (queue: Queue) (cosmosClient: CosmosClient) =
        Container.upsert queue (PartitionKey queue.Id) (container cosmosClient)
        
module Match =
    let [<Literal>] CONTAINER_NAME = "matches"

    let container (cosmosClient: CosmosClient) =
        cosmosClient.GetContainer(DATABASE_NAME, CONTAINER_NAME)

    let get (matchId: string) (cosmosClient: CosmosClient) =
        Container.read<Match> matchId (PartitionKey matchId) (container cosmosClient)

    let set (match': Match) (cosmosClient: CosmosClient) =
        Container.upsert match' (PartitionKey match'.Id) (container cosmosClient)
