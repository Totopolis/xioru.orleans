using Orleans.Concurrency;
using System.CommandLine;
using System.Text.RegularExpressions;
using Xioru.Messaging.Contracts.Channel;
using Xioru.Messaging.Contracts.Command;
using Xioru.Messaging.Contracts.CommandExecutor;

namespace Xioru.Messaging.CommandExecutor
{
    // TODO: count of workers depends of active users count
    [StatelessWorker(2)]
    public class CommandExecutorGrain :
        Orleans.Grain,
        ICommandExecutor
    {
        private readonly Dictionary<string, IChannelCommand> _commands;

        public CommandExecutorGrain(IEnumerable<IChannelCommand> commands)
        {
            _commands = commands.ToDictionary(x => x.Command.Name);
        }

        public async Task<CommandResult> Execute(
            Guid projectId,
            Guid channelId,
            bool isSupervisor,
            string commandText)
        {
            if (string.IsNullOrWhiteSpace(commandText))
            {
                return CommandResult.LogicError("Empty command");
            }

            if (commandText.Contains(Environment.NewLine))
            {
                return CommandResult.LogicError("Multiline command not supported");
            }

            if (!commandText.StartsWith('/'))
            {
                // probably got a user message
                return CommandResult.Success(string.Empty);
            }

            var cleanCommand = commandText.TrimStart('/');
            var commandName = Regex.Match(cleanCommand, @"^\w+").Value;

            if (!_commands.TryGetValue(commandName, out var command))
            {
                return CommandResult.SyntaxError("Unknown command\nSee /help");
            }

            var parseResult = command.Command.Parse(cleanCommand);
            if (parseResult.Errors.Any())
            {
                var msg = string.Join(';', parseResult.Errors.Select(x => x.Message));
                return CommandResult.SyntaxError(msg);
            }

            var context = new ChannelCommandContext()
            {
                IsSupervisor = isSupervisor,
                ProjectId = projectId,
                ChannelId = channelId,
                ParsedCommand = parseResult
            };

            var result = await command.Execute(context);
            return result;
        }

        private static IReadOnlyList<string> SplitArguments(string commandText)
        {
            const char divider = '\"';

            var quoteDevidedSegments = commandText.Split(divider);
            if (quoteDevidedSegments.Length % 2 == 0)
            {
                throw new ArgumentException("Missing symbol '\"'");
            }

            var ret = new List<string>();
            for(int i = 0; i < quoteDevidedSegments.Length; i++)
            {
                var segment = quoteDevidedSegments[i];
                if (i % 2 == 0)
                {
                    ret.AddRange(segment.Split(' ')
                        .Where(x => !string.IsNullOrWhiteSpace(x))); 
                }
                else
                {
                    ret.Add(segment);
                }
            }

            return ret;
        }
    }
}
