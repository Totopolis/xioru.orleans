namespace Xioru.Messaging.Contracts.Command;

public class CommandContext
{
    public bool IsSupervisor { get; set; } = false;

    public string CommandText { get; set; } = default!;
}
