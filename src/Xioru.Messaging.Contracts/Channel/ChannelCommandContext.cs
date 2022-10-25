using Xioru.Messaging.Contracts.Command;

namespace Xioru.Messaging.Contracts.Channel;

public class ChannelCommandContext : CommandContext
{
    public Guid ProjectId { get; set; } = default;

    public Guid ChannelId { get; set; } = default;
}
