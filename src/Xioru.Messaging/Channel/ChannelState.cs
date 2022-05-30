using Xioru.Grain.AbstractGrain;
using Xioru.Messaging.Contracts.Messenger;

namespace Xioru.Messaging
{
    public class ChannelState : AbstractGrainState
    {
        public MessengerType MessengerType { get; set; }

        public Guid MessengerId { get; set; }

        public string ChatId { get; set; } = default!;

        public DateTime LastMessage { get; set; } = DateTime.MinValue;
    }
}
