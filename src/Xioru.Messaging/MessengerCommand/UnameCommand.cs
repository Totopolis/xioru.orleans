using Microsoft.Extensions.DependencyInjection;
using Orleans;
using System.CommandLine;
using System.Text;
using Xioru.Grain.Contracts;
using Xioru.Messaging.Contracts.Command;
using Xioru.Messaging.Contracts.Messenger;
using Xioru.Messaging.Messenger;

namespace Xioru.Messaging.MessengerCommand
{
    public class UnameCommand : AbstractMessengerCommand
    {
        private readonly IVersionProvider? _versionProvider;

        public UnameCommand(
            IGrainFactory factory,
            IServiceProvider services) : base(factory)
        {
            _versionProvider = services.GetService<IVersionProvider>();
        }

        protected override Command Command => new Command(
            "uname", "display system info");

        protected override Task<CommandResult> ExecuteInternal(MessengerCommandContext context)
        {
            if (_versionProvider == null)
            {
                return Task.FromResult(
                    CommandResult.InternalError("No version provider provided"));
            }

            var version = _versionProvider.GetVersion();

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
