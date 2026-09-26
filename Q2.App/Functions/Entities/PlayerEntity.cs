using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Entities;
using System.Text.Json.Serialization;
using System.Threading.Channels;

namespace Q2.App.Functions.Entities;

public enum Rank
{
    Bronze,
    Silver,
    Gold,
    Platinum,
    Diamond,
    Champion,
    GrandChampion,
}

public class PlayerState
{
    public string Id { get; set; }

    public string Ign { get; set; }

    public int Elo { get; set; }

    public string? CurrentMatchId { get; set; }

    public string? CurrentQueueId { get; set; }

    [JsonConstructor]
    public PlayerState(string id, string ign, int elo, string? currentMatchId, string? currentQueueId)
    {
        Id = id;
        Ign = ign;
        Elo = elo;
        CurrentMatchId = currentMatchId;
        CurrentQueueId = currentQueueId;
    }

    public PlayerState(string id, string ign, Rank rank, string? currentMatchId, string? currentQueueId)
    {
        Id = id;
        Ign = ign;
        Elo = GetRankDefaultElo(rank);
        CurrentMatchId = currentMatchId;
        CurrentQueueId = currentQueueId;
    }

    public void SetIgn(string ign)
    {
        Ign = ign;
    }

    public void SetElo(int elo)
    {
        Elo = elo;
    }

    public void JoinQueue(string queueId)
    {
        if (CurrentQueueId is not null)
            throw new PlayerAlreadyInQueueException();

        if (CurrentMatchId is not null)
            throw new PlayerAlreadyInMatchException();

        CurrentQueueId = queueId;
    }

    public void LeaveQueue()
    {
        if (CurrentQueueId is null)
            throw new PlayerNotInQueueException();

        CurrentQueueId = null;
    }

    public void SetMatch(string? matchId)
    {
        CurrentQueueId = null;
        CurrentMatchId = matchId;
    }

    public bool CanQueue() => CurrentQueueId is null && CurrentMatchId is null;

    private static int GetRankDefaultElo(Rank rank) => rank switch
    {
        Rank.Bronze or Rank.Silver or Rank.Gold => 400,
        Rank.Platinum => 600,
        Rank.Diamond => 800,
        Rank.Champion => 1000,
        Rank.GrandChampion => 1200,
        _ => throw new InvalidOperationException($"Invalid rank: {rank}"),
    };
}

public class PlayerDoesNotExistException : Exception { }

public class PlayerAlreadyExistsException : Exception { }

public class PlayerAlreadyInQueueException : Exception { }

public class PlayerNotInQueueException : Exception { }

public class PlayerAlreadyInMatchException : Exception { }

public class PlayerNotInMatchException : Exception { }

public record PlayerCreateInput(string Id, string Ign, Rank Rank);

public record PlayerSetMatchInput(string? MatchId);

public class PlayerEntity : TaskEntity<PlayerState?>
{
    public static EntityInstanceId Id(string playerId) => new(nameof(PlayerEntity), playerId);

    protected override PlayerState? InitializeState(TaskEntityOperation _) => null;

    public PlayerState? GetState() => State;

    public PlayerState Create(PlayerCreateInput input)
    {
        if (State is not null)
            throw new PlayerAlreadyExistsException();

        State = new PlayerState(input.Id, input.Ign, input.Rank, null, null);
        return State;
    }

    public PlayerState SetIgn(string ign)
    {
        if (State is null)
            throw new PlayerDoesNotExistException();

        State.SetIgn(ign);
        return State;
    }

    public PlayerState SetElo(int elo)
    {
        if (State is null)
            throw new PlayerDoesNotExistException();

        State.SetElo(elo);
        return State;
    }

    public PlayerState JoinQueue(string queueId)
    {
        if (State is null)
            throw new PlayerDoesNotExistException();

        State.JoinQueue(queueId);
        return State;
    }

    public PlayerState LeaveQueue()
    {
        if (State is null)
            throw new PlayerDoesNotExistException();

        State.LeaveQueue();
        return State;
    }

    public PlayerState SetMatch(PlayerSetMatchInput input)
    {
        if (State is null)
            throw new PlayerDoesNotExistException();

        State.SetMatch(input.MatchId);
        return State;
    }

    [Function(nameof(PlayerEntity))]
    public Task RunEntityAsync([EntityTrigger] TaskEntityDispatcher dispatcher) =>
        dispatcher.DispatchAsync(this);
}

public static class PlayerEntityExtensions
{
    public static async Task<PlayerState?> PlayerGetStateAsync(
        this TaskOrchestrationContext ctx,
        string playerId)
    {
        return await ctx.Entities.CallEntityAsync<PlayerState?>(
            PlayerEntity.Id(playerId),
            nameof(PlayerEntity.GetState));
    }

    public static async Task<PlayerState> PlayerCreateAsync(
        this TaskOrchestrationContext ctx,
        string playerId,
        string ign,
        Rank rank)
    {
        return await ctx.Entities.CallEntityAsync<PlayerState>(
            PlayerEntity.Id(playerId),
            nameof(PlayerEntity.Create),
            new PlayerCreateInput(playerId, ign, rank));
    }

    public static async Task<PlayerState> PlayerSetIgnAsync(
        this TaskOrchestrationContext ctx,
        string playerId,
        string ign)
    {
        return await ctx.Entities.CallEntityAsync<PlayerState>(
            PlayerEntity.Id(playerId),
            nameof(PlayerEntity.SetIgn),
            ign);
    }

    public static async Task<PlayerState> PlayerSetEloAsync(
        this TaskOrchestrationContext ctx,
        string playerId,
        int elo)
    {
        return await ctx.Entities.CallEntityAsync<PlayerState>(
            PlayerEntity.Id(playerId),
            nameof(PlayerEntity.SetElo),
            elo);
    }

    public static async Task<PlayerState> PlayerJoinQueueAsync(
        this TaskOrchestrationContext ctx,
        string playerId,
        string queueId)
    {
        return await ctx.Entities.CallEntityAsync<PlayerState>(
            PlayerEntity.Id(playerId),
            nameof(PlayerEntity.JoinQueue),
            queueId);
    }

    public static async Task<PlayerState> PlayerLeaveQueueAsync(
        this TaskOrchestrationContext ctx,
        string playerId)
    {
        return await ctx.Entities.CallEntityAsync<PlayerState>(
            PlayerEntity.Id(playerId),
            nameof(PlayerEntity.LeaveQueue));
    }

    public static async Task<PlayerState> PlayerSetMatchAsync(
        this TaskOrchestrationContext ctx,
        string playerId,
        string? matchId)
    {
        return await ctx.Entities.CallEntityAsync<PlayerState>(
            PlayerEntity.Id(playerId),
            nameof(PlayerEntity.SetMatch),
            new PlayerSetMatchInput(matchId));
    }
    
    public static async Task PlayerDeleteAsync(
        this TaskOrchestrationContext ctx,
        string playerId)
    {
        await ctx.Entities.CallEntityAsync(
            PlayerEntity.Id(playerId),
            "delete");
    }
}
