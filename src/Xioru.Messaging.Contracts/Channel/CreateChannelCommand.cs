using Xioru.Grain.Contracts.AbstractGrain;
using Xioru.Messaging.Contracts.Messenger;

namespace Xioru.Messaging.Contracts.Channel
{
    public record CreateChannelCommand(
        Guid ProjectId,
        string Name,
        string DisplayName,
        string Description,
        string[] Tags,
        //
        MessengerType MessengerType,
        Guid MessengerId,
        string ChatId) : CreateAbstractGrainCommand(
            ProjectId,
            Name,
            DisplayName,
            Description,
            Tags);
}