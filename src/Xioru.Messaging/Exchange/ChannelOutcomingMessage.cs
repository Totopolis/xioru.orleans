using Xioru.Messaging.Contracts.Messenger;

namespace Xioru.Messaging.Contracts.Channel
{
    public class ChannelOutcomingMessage
    {
        public MessengerType MessengerType { get; set; } = default(MessengerType);

        public string ChatId { get; set; } = default!;

        public string Message { get; set; } = default!;
    }
}
