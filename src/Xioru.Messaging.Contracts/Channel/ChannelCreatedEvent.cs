using Xioru.Grain.Contracts.AbstractGrain;
using Xioru.Messaging.Contracts.Messenger;

namespace Xioru.Messaging.Contracts.Channel
{
    public record ChannelCreatedEvent(
        string DisplayName,
        string Description,
        string[] Tags,
        //
        MessengerType MessengerType,
        Guid MessengerId,
        string ChatId) : AbstractGrainCreatedEvent(
            DisplayName,
            Description,
            Tags);
}
