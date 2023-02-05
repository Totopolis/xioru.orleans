namespace Xioru.Messaging.Contracts.Channel;

[GenerateSerializer]
public class ChannelIncomingMessage
{
    [Id(0)]
    public DateTime Created { get; set; } = DateTime.Now;

    [Id(1)]
    public string Text { get; set; } = default!;

    [Id(2)]
    public string UserName { get; set; } = default!;

    [Id(3)]
    public bool IsSupervisor { get; set; } = false;
}
