using Xioru.Grain.Contracts.AbstractGrain;
using Xioru.Messaging.Contracts.Messenger;

namespace Xioru.Messaging.Contracts.Channel
{
    public record ChannelProjection(
        string Name,
        Guid ProjectId,
        string DisplayName,
        string Description,
        string[] Tags,
        //
        MessengerType MessengerType,
        Guid MessengerId,
        string ChatId,
        DateTime LastMessage) : AbstractGrainProjection(
            Name,
            ProjectId,
            DisplayName,
            Description,
            Tags);
}
