using Xioru.Grain.Contracts.Messages;

namespace Xioru.Messaging.Contracts.Channel.Events;

[GenerateSerializer]
public record ChannelDeletedEvent : GrainDeletedEvent
{
}
