using System.CommandLine.Parsing;

namespace Xioru.Messaging.Contracts.Command
{
    public class CommandContext
    {
        public bool IsSupervisor { get; set; } = false;

        public ParseResult Result { get; set; } = default!;
    }
}
