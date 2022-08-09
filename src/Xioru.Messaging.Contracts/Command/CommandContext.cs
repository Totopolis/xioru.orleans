using System.CommandLine;
using System.CommandLine.Parsing;

namespace Xioru.Messaging.Contracts.Command
{
    public class CommandContext
    {
        public bool IsSupervisor { get; set; } = false;

        public ParseResult ParsedCommand { get; set; } = default!;

        public T GetArgumentValue<T>(Argument<T> argument)
        {
            return ParsedCommand.GetValueForArgument(argument);
        }

        public T? GetOptionValue<T>(Option<T> option)
        {
            return ParsedCommand.GetValueForOption(option);
        }
    }
}
