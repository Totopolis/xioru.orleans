namespace Xioru.Messaging.Contracts.Command
{
    public class CommandContext
    {
        public bool IsSupervisor { get; set; } = false;

        public string[] Arguments { get; set; } = default!;

        public int ArgsCount => Arguments.Length;
    }
}
