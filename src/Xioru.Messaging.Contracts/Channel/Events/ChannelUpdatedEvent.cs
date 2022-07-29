using Xioru.Grain.Contracts.AbstractGrain;

namespace Xioru.Messaging.Contracts.Channel.Events
{
    public record ChannelUpdatedEvent(
        string DisplayName,
        string Description,
        string[] Tags) : AbstractGrainUpdatedEvent(
            DisplayName,
            Description,
            Tags);
}
