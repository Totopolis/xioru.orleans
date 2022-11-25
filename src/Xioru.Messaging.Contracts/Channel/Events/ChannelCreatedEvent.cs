using Xioru.Grain.Contracts.AbstractGrain;
using Xioru.Messaging.Contracts.Messenger;

namespace Xioru.Messaging.Contracts.Channel.Events;

[GenerateSerializer]
public record ChannelCreatedEvent(
    string DisplayName,
    string Description,
    string[] Tags,
    //
    MessengerType MessengerType,
    string ChatId) : AbstractGrainCreatedEvent(
        DisplayName,
        Description,
        Tags);
