using Q2.App.Common;

namespace Q2.App.Callbacks;

public record PlayerMatchResult(string Id, int Elo, int EloChange);

public class MatchCompletedCallback(IList<PlayerMatchResult> players, int team1Score, int team2Score) : ICallback
{
    public IList<PlayerMatchResult> Players { get; } = players.OrderBy(p => p.Id).ToList();

    public int Team1Score { get; } = team1Score;

    public int Team2Score { get; } = team2Score;

    public MessageInteractionCallback ToCallback()
    {
        var components = new List<Component>
        {
            new TextDisplayComponent($"Score successfully reported as **{Team1Score}-{Team2Score}**."),
            new SeparatorComponent(divider: false, SeparatorComponentSpacing.Small),
        };

        foreach (var player in Players)
        {
            var mention = $"<@{player.Id}>";
            var emoji = player.EloChange switch
            {
                < 0 => "🔻",
                > 0 => "🔺",
                0 => "🔹",
            };

            components.Add(new TextDisplayComponent($"{emoji} {mention} - {player.Elo} ({player.EloChange:+#;-#;0})"));
        }

        components.AddRange([
            new SeparatorComponent(false, SeparatorComponentSpacing.Small),
            new TextDisplayComponent($"You can now re-queue for a new match using the `/queue` command."),
            new SeparatorComponent(false, SeparatorComponentSpacing.Small),
            new SeparatorComponent(divider: true, SeparatorComponentSpacing.Small),
            new TextDisplayComponent($"-# {(string.Join(' ', Players.Select(p => $"<@{p.Id}>")))}"),
        ]);

        return new(MessageFlags.IsComponentsV2, [new ContainerComponent(components)]);
    }
}
