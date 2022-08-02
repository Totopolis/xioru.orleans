using Xioru.Messaging.Contracts.Command;

namespace Xioru.Messaging.Contracts.Messenger
{
    public class MessengerCommandContext : CommandContext
    {
        public IMessengerRepository Manager { get; set; } = default!;

        public string ChatId { get; set; } = default!;

        public MessengerType MessengerType { get; set; } = default!;
    }
}
