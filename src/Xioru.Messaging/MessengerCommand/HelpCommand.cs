using Microsoft.Extensions.DependencyInjection;
using Orleans;
using System.CommandLine;
using System.Text;
using Xioru.Messaging.Contracts.Channel;
using Xioru.Messaging.Contracts.Command;
using Xioru.Messaging.Contracts.Messenger;
using Xioru.Messaging.Messenger;

namespace Xioru.Messaging.MessengerCommand
{
    public class HelpCommand : AbstractMessengerCommand
    {
        private readonly IServiceProvider _services;

        private readonly Argument<string?> _nameArgument = new Argument<string?>(
                name: "command-name",
                description: "name of the available command",
                getDefaultValue: () => null);

        public HelpCommand(
            IGrainFactory factory,
            IServiceProvider serviceProvider) : base(factory)
        {
            _services = serviceProvider;
        }

        protected override Command Command => new Command(
            "help", "system help")
        {
            _nameArgument
        };

        protected override Task<CommandResult> ExecuteInternal(MessengerCommandContext context)
        {
            var messengerCommands = _services.GetRequiredService<IEnumerable<IMessengerCommand>>();
            var channelCommands = _services.GetRequiredService<IEnumerable<IChannelCommand>>();

            var commandName = GetArgumentValue(_nameArgument);

            var sb = new StringBuilder();

            if (string.IsNullOrWhiteSpace(commandName))
            {
                sb.AppendLine("Common commands:");
                messengerCommands
                    .ToList()
                    .ForEach(x =>
                    {
                        sb.Append(x.Name);
                        sb.Append(": ");
                        sb.AppendLine(x.Description);
                    });

                sb.AppendLine();
                sb.AppendLine("Project commands:");
                channelCommands
                    .ToList()
                    .ForEach(x =>
                    {
                        sb.Append(x.Name);
                        sb.Append(": ");
                        sb.AppendLine(x.Description);
                    });
            }
            else
            {
                ICommand? command = messengerCommands.FirstOrDefault(x => x.Name == commandName) as ICommand ??
                    channelCommands.FirstOrDefault(x => x.Name == commandName) ?? null;

                if (command != null)
                {
                    sb.AppendLine(command.GetHelp());
                }
                else
                {
                    sb.AppendLine("Command not found");
                }
            }

            return Task.FromResult(CommandResult.Success(sb.ToString()));
        }
    }
}
