namespace Xioru.Iot.Domain.DiscordBot;

internal class _Record
{
    public long ChannelId { get; set; }
    public string ProjectId { get; set; } = default!;

    // claims:
    // 1. viewer - only subscribe on log and alerts + see state
    // 2. manager - viewer + ...
    // 3. dispatcher - manager + send commands + quitirovanie alerts
    // 4. engineer - dispatcher + modify model + crud objects
    // 5. owner - engineer + manage rights + billing
}
