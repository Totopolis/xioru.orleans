using Xioru.Messaging.Contracts.Formatting;
using Xioru.Messaging.Contracts.Messenger;

namespace Xioru.Messaging.Contracts.Channel
{
    public class ChannelOutcomingMessage
    {
        public string ChatId { get; set; } = default!;

        public FormattedString Message { get; set; } = default!;
    }
}
