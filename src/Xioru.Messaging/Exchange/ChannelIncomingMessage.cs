namespace Xioru.Messaging.Contracts.Channel;

public class ChannelIncomingMessage
{
    public DateTime Created { get; set; } = DateTime.Now;

    public string Text { get; set; } = default!;

    public string UserName { get; set; } = default!;

    public bool IsSupervisor { get; set; } = false;
}
