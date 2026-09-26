using Q2.App.Common;
using Q2.App.Functions.Activities;

namespace Q2.App.Callbacks;

public class MatchStartedCallback(Team team1, Team team2) : ICallback
{
    public Team Team1 { get; } = team1;

    public Team Team2 { get; } = team2;

    public MessageInteractionCallback ToCallback()
    {
        var probability = Elo.Expected(Team1.AverageElo, Team2.AverageElo);
        var probabilityPercentage = (int)(probability * 100);

        var expectedWinner = probability > 0.5 ? 1 : 2;
        var expectedPercentage = expectedWinner == 1 ? probabilityPercentage : 100 - probabilityPercentage;

        return new(
            MessageFlags.IsComponentsV2,
            [
                new ContainerComponent([
                    new TextDisplayComponent("Match started! Party up to start your party match. The teams are:"),
                    new SeparatorComponent(divider: false, SeparatorComponentSpacing.Small),
                    new TextDisplayComponent($"**Team 1:** {string.Join(' ', Team1.PlayerIds.Select(id => $"<@{id}>"))}"),
                    new TextDisplayComponent($"**Team 2:** {string.Join(' ', Team2.PlayerIds.Select(id => $"<@{id}>"))}"),
                    new SeparatorComponent(divider: false, SeparatorComponentSpacing.Small),
                    new TextDisplayComponent($"The expected winner is **Team {expectedWinner}** with a probability of **{expectedPercentage}%**."),
                    new SeparatorComponent(divider: false, SeparatorComponentSpacing.Small),
                    new TextDisplayComponent("Report the game after completing a best of 3 series through the `/report` command. Good luck!"),
                    new SeparatorComponent(divider: false, SeparatorComponentSpacing.Small),
                    new SeparatorComponent(divider: true, SeparatorComponentSpacing.Small),
                    new TextDisplayComponent($"-# {(string.Join(' ', Team1.PlayerIds.Concat(Team2.PlayerIds).Order().Select(id => $"<@{id}>")))}"),
                ]),
            ]);
    }
}
