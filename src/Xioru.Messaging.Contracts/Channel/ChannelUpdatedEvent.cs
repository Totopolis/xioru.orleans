using Xioru.Grain.Contracts.AbstractGrain;

namespace Xioru.Messaging.Contracts.Channel
{
    public record ChannelUpdatedEvent(
        string DisplayName,
        string Description,
        string[] Tags) : AbstractGrainUpdatedEvent(
            DisplayName,
            Description,
            Tags);
}
