using Xioru.Messaging.Contracts.Formatting;

namespace Xioru.Messaging.Contracts.Channel;

[GenerateSerializer]
public class ChannelOutcomingMessage
{
    public string ChatId { get; set; } = default!;

    public FormattedString Message { get; set; } = default!;
}
