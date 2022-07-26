using Orleans.Concurrency;
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
            _commands = new Dictionary<string, IChannelCommand>();
            var array = commands.ToArray();
            array
                .Where(x => x.IsSubCommandExists)
                .ToList()
                .ForEach(x => _commands.Add($"{x.CommandName}.{x.SubCommandName}", x));

            array
                .Where(x => !x.IsSubCommandExists)
                .ToList()
                .ForEach(x => _commands.Add($"{x.CommandName}", x));
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
            var segments = SplitArguments(commandText);

            if (segments == null || segments.Count <= 0 || !segments![0].StartsWith('/'))
            {
                return CommandResult.Success(string.Empty);
            }

            var context = new ChannelCommandContext()
            {
                IsSupervisor = isSupervisor,
                ProjectId = projectId,
                ChannelId = channelId
            };

            var segment0 = segments[0].TrimStart('/');

            if (segments.Count >= 2 &&
                _commands.TryGetValue($"{segment0}.{segments[1].ToLower()}", out var command))
            {
                context.Arguments = segments.Skip(2).ToArray();
            }
            else if (_commands.TryGetValue($"{segment0}", out command))
            {
                context.Arguments = segments.Skip(1).ToArray();
            }
            else
            {
                return CommandResult.SyntaxError("Unknown command\nSee /help");
            }

            if (command == null)
            {
                return CommandResult.SyntaxError("Unknown command\nSee /help");
            }

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
