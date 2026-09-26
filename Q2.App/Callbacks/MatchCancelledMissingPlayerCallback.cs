using Q2.App.Common;

namespace Q2.App.Callbacks;

public class MatchCancelledMissingPlayerCallback(IList<string> playerIds) : ICallback
{
    public IList<string> PlayerIds { get; } = playerIds.Order().ToList();

    public MessageInteractionCallback ToCallback()
    {
        return new(
            MessageFlags.IsComponentsV2,
            [
                new ContainerComponent([
                    new TextDisplayComponent("Your match has been cancelled due to a player no longer being able to play. Please use `/queue` to requeue a new match."),
                    new SeparatorComponent(divider: false, SeparatorComponentSpacing.Small),
                    new SeparatorComponent(divider: true, SeparatorComponentSpacing.Small),
                    new TextDisplayComponent($"-# {(string.Join(' ', PlayerIds.Select(id => $"<@{id}>")))}"),
                ]),
            ]);
    }
}
