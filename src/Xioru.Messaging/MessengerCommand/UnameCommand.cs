using Orleans;
using System.CommandLine;
using System.Reflection;
using System.Text;
using Xioru.Messaging.Contracts.Command;
using Xioru.Messaging.Contracts.Messenger;
using Xioru.Messaging.Messenger;

namespace Xioru.Messaging.MessengerCommand
{
    public class UnameCommand : BaseMessengerCommand
    {
        public UnameCommand(IGrainFactory factory) : base(factory)
        {
        }

        public override Command Command => new Command(
            "uname", "display system info");

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
