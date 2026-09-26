using Q2.App.Common;

namespace Q2.App.Callbacks;

public class QueueFilledCallback(
    string matchId,
    IDictionary<string, bool> playersReady,
    DateTimeOffset timeout,
    int timeoutMinutes) : ICallback
{
    public string MatchId { get; set; } = matchId;

    public IDictionary<string, bool> PlayersReady { get; set; } = playersReady;

    public DateTimeOffset Timeout { get; set; } = timeout;

    public int TimeoutMinutes { get; set; } = timeoutMinutes;

    public MessageInteractionCallback ToCallback()
    {
        var components = new List<Component>
        {
            new TextDisplayComponent($"Your queue has filled! Press the ready button below. If not all players ready up in {TimeoutMinutes} minutes, the game will be abandoned."),
            new SeparatorComponent(false, SeparatorComponentSpacing.Small),
        };

        foreach (var kvp in PlayersReady)
        {
            var emoji = kvp.Value ? "✅" : "❎";
            var mention = $"<@{kvp.Key}>";
            components.Add(new TextDisplayComponent($"{emoji} {mention}"));
        }

        components.AddRange([
            new SeparatorComponent(false, SeparatorComponentSpacing.Small),
            new TextDisplayComponent($"If not all players ready up, the match with be abandoned in <t:{Timeout.ToUnixTimeSeconds()}:R>"),
            new SeparatorComponent(false, SeparatorComponentSpacing.Small),
            new ActionRowComponent([
                new ButtonComponent("Ready up", $"ready-{MatchId}", disabled: false),
            ]),
            new SeparatorComponent(false, SeparatorComponentSpacing.Small),
            new SeparatorComponent(divider: true, SeparatorComponentSpacing.Small),
            new TextDisplayComponent($"-# {(string.Join(' ', PlayersReady.Keys.Select(id => $"<@{id}>")))}"),
        ]);

        return new(MessageFlags.IsComponentsV2, [new ContainerComponent(components)]);
    }
}
