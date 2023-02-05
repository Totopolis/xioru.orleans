using Xioru.Messaging.Contracts.Formatting;

namespace Xioru.Messaging.Contracts.Channel;

[GenerateSerializer]
public class ChannelOutcomingMessage
{
    [Id(0)]
    public string ChatId { get; set; } = default!;

    
    [Id(1)]
    public FormattedString Message { get; set; } = default!;
}
