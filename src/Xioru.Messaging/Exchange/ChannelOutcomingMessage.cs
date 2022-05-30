namespace Xioru.Messaging.Contracts.Channel
{
    public class ChannelOutcomingMessage
    {
        public string ChatId { get; set; } = default!;

        public string Message { get; set; } = default!;
    }
}
