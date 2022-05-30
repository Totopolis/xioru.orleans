using Orleans;
using System.Reflection;
using System.Text;
using Xioru.Messaging.Contracts.Command;
using Xioru.Messaging.Contracts.Messenger;
using Xioru.Messaging.Messenger;

namespace Xioru.Messaging.MessengerCommand
{
    public class UnameCommand : BaseMessengerCommand
    {
        public const string UsageConst = "/uname";

        public UnameCommand(IGrainFactory factory) : base(
            factory: factory,
            commandName: "uname",
            subCommandName: String.Empty,
            minArgumentsCount: 0,
            maxArgumentsCount: 0,
            usage: UsageConst)
        {
        }

        protected override Task<CommandResult> ExecuteInternal(MessengerCommandContext context)
        {
            var version = Assembly
                .GetExecutingAssembly()
                .GetName()
                .Version!;

            var sb = new StringBuilder();
            sb.Append(version.Major);
            sb.Append('.');
            sb.Append(version.Minor);
            sb.Append('.');
            sb.Append(version.Build);

            return Task.FromResult(CommandResult.Success(sb.ToString()));
        }
    }
}
