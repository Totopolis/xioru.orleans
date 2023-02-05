using Xioru.Grain.AbstractGrain;
using Xioru.Messaging.Contracts.Messenger;

namespace Xioru.Messaging;

[GenerateSerializer]
public class ChannelState : AbstractGrainState
{
    [Id(0)]
    public MessengerType MessengerType { get; set; }

    [Id(1)]
    public string ChatId { get; set; } = default!;

    [Id(2)]
    public DateTime LastMessage { get; set; } = DateTime.MinValue;
}
