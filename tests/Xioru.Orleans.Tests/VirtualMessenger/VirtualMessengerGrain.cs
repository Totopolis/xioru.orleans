using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xioru.Messaging.Contracts.Command;
using Xioru.Messaging.Contracts.Formatting;
using Xioru.Messaging.Contracts.Messenger;
using MicrosoftGrain = Orleans.Grain;

namespace Xioru.Orleans.Tests.VirtualMessenger;

public class VirtualMessengerGrain :
    MicrosoftGrain,
    IVirtualMessengerGrain
{
    private readonly IMessengerRepository _repository;
    private readonly Dictionary<string, IMessengerCommand> _commands;

    public VirtualMessengerGrain(
        IMessengerRepository repository,
        IEnumerable<IMessengerCommand> commands)
    {
        _commands = commands.ToDictionary(x => x.Name);
        _repository = repository;
    }

    public async Task<CommandResult> ExecuteSupervisorCommand(
        string commandText)
    {
        if (!commandText.StartsWith('/'))
        {
            throw new ArgumentException();
        }

        commandText = commandText.TrimStart('/');
        var commandName = Regex.Match(commandText, @"^([\w\-]+)").Value;

        if (!_commands.TryGetValue(commandName, out var command))
        {
            throw new ArgumentException();
        }

        var context = new MessengerCommandContext()
        {
            IsSupervisor = true,
            ChatId = "1234",
            Manager = _repository,
            MessengerType = MessengerType.Virtual,
            CommandText = commandText
        };

        var result = await command.Execute(context);

        return result;
    }

    public async Task StartAsync()
    {
        await _repository.StartAsync(MessengerType.Virtual);
    }

    public Task SendDirectMessage(
        string chatId,
        FormattedString message) => throw new NotImplementedException();
}
