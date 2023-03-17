using Microsoft.Extensions.DependencyInjection;
using System.CommandLine;
using Xioru.Messaging.Contracts.Channel;
using Xioru.Messaging.Contracts.Command;
using Xioru.Messaging.Contracts.Formatting;
using Xioru.Messaging.Contracts.Messenger;
using Xioru.Messaging.Messenger;

namespace Xioru.Messaging.MessengerCommand;

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

        var result = new FormattedString();

        if (string.IsNullOrWhiteSpace(commandName))
        {
            result.Append("Common commands:\n");
            messengerCommands
                .OrderBy(x => x.Name)
                .ToList()
                .ForEach(x =>
                {
                    result.Append(x.Name, StringFormatting.Bold);
                    result.Append(": ");
                    result.Append(x.Description);
                    result.Append("\n");
                });

            result.Append("\n");
            result.Append("Project commands:\n");
            channelCommands
                .OrderBy(x => x.Name)
                .ToList()
                .ForEach(x =>
                {
                    result.Append(x.Name, StringFormatting.Bold);
                    result.Append(": ");
                    result.Append(x.Description);
                    result.Append("\n");
                });
        }
        else
        {
            ICommand? command = messengerCommands.FirstOrDefault(x => x.Name == commandName) as ICommand ??
                channelCommands.FirstOrDefault(x => x.Name == commandName) ?? null;

            if (command != null)
            {
                result.Append(command.GetHelp());
                result.Append("\n");
            }
            else
            {
                result.Append("Command not found");
                result.Append("\n");
            }
        }

        return Task.FromResult(CommandResult.Success(result));
    }
}
