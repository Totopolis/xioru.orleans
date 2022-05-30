using ConsoleTables;
using Orleans;
using System.Text;
using Xioru.Messaging.Contracts.Command;
using Xioru.Messaging.Contracts.Messenger;
using Xioru.Messaging.Messenger;

namespace Xioru.Messaging.MessengerCommand
{
    public class PwdCommand : BaseMessengerCommand
    {
        public const string UsageConst = "/pwd";

        public PwdCommand(IGrainFactory factory) : base(
            factory: factory,
            commandName: "pwd",
            subCommandName: String.Empty,
            minArgumentsCount: 0,
            maxArgumentsCount: 0,
            usage: UsageConst)
        {
        }

        protected override Task<CommandResult> ExecuteInternal(MessengerCommandContext context)
        {
            if (!context.Manager.TryGetChannels(context.ChatId, out var channels))
            {
                return Task.FromResult(CommandResult.Success("No accessed projects"));
            }

            var sb = new StringBuilder();
            sb.AppendLine("> List of the accessible projects");
            sb.Append("```");

            var table = new ConsoleTable("Name", "Current");

            foreach (var it in channels)
            {
                table.AddRow(it.ProjectName, it.IsCurrent ? "X" : string.Empty);
            }

            sb.Append($"{table.ToMinimalString()}```");

            return Task.FromResult(CommandResult.Success(sb.ToString()));
        }
    }
}
