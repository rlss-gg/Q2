using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Entities;

namespace Q2.App.Functions.Entities;

public class QueueState
{
    public string ChannelId { get; set; }

    public int Size { get; set;  }

    public HashSet<string> Players { get; set; }

    public QueueState(string channelId, int size, HashSet<string> players)
    {
        ChannelId = channelId;
        Size = size;
        Players = players;
    }
}

public record QueueCreateInput(string ChannelId, int Size);

public class QueueEntity : TaskEntity<QueueState?>
{
    public static EntityInstanceId Id(string channelId) => new(nameof(QueueEntity), channelId);

    protected override QueueState? InitializeState(TaskEntityOperation _) => null;

    public QueueState? GetState() => State;

    public QueueState Create(QueueCreateInput input)
    {
        if (State is not null)
            throw new InvalidOperationException("Queue already exists");

        if (input.Size < 2)
            throw new InvalidOperationException("Queue size too small");

        if (input.Size > 6)
            throw new InvalidOperationException("Queue size too large");

        if (input.Size % 2 != 0)
            throw new InvalidOperationException("Queue size not even");

        State = new(input.ChannelId, input.Size, []);
        return State;
    }

    public QueueState Enqueue(string playerId)
    {
        if (State is null)
            throw new InvalidOperationException("Queue does not exist");

        var success = State.Players.Add(playerId);

        if (!success)
            throw new InvalidOperationException("Player already in queue");

        if (State.Players.Count != State.Size)
            return State;

        var players = new HashSet<string>(State.Players);
        State.Players.Clear();

        return new QueueState(State.ChannelId, State.Size, players);
    }

    public QueueState Dequeue(string playerId)
    {
        if (State is null)
            throw new InvalidOperationException("Queue does not exist");

        var success = State.Players.Remove(playerId);

        if (!success)
            throw new InvalidOperationException("Player not in queue");

        return State;
    }

    [Function(nameof(QueueEntity))]
    public Task RunEntityAsync([EntityTrigger] TaskEntityDispatcher dispatcher) =>
        dispatcher.DispatchAsync(this);
}

public static class QueueEntityExtensions
{
    public static async Task<QueueState?> QueueGetStateAsync(
        this TaskOrchestrationContext ctx,
        string channelId)
    {
        return await ctx.Entities.CallEntityAsync<QueueState?>(
            QueueEntity.Id(channelId),
            nameof(QueueEntity.GetState));
    }

    public static async Task<QueueState> QueueCreateAsync(
        this TaskOrchestrationContext ctx,
        string channelId,
        int size)
    {
        return await ctx.Entities.CallEntityAsync<QueueState>(
            QueueEntity.Id(channelId),
            nameof(QueueEntity.Create),
            new QueueCreateInput(channelId, size));
    }

    public static async Task<QueueState> QueueEnqueueAsync(
        this TaskOrchestrationContext ctx,
        string channelId,
        string playerId)
    {
        return await ctx.Entities.CallEntityAsync<QueueState>(
            QueueEntity.Id(channelId),
            nameof(QueueEntity.Enqueue),
            playerId);
    }

    public static async Task<QueueState> QueueDequeueAsync(
        this TaskOrchestrationContext ctx,
        string channelId,
        string playerId)
    {
        return await ctx.Entities.CallEntityAsync<QueueState>(
            QueueEntity.Id(channelId),
            nameof(QueueEntity.Dequeue),
            playerId);
    }

    public static async Task QueueDeleteAsync(
        this TaskOrchestrationContext ctx,
        string channelId)
    {
        await ctx.Entities.CallEntityAsync(
            QueueEntity.Id(channelId),
            "delete");
    }
}
