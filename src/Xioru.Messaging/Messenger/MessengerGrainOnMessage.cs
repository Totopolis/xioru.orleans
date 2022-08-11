using System.Text.RegularExpressions;
using Xioru.Messaging.Contracts.Channel;
using Xioru.Messaging.Contracts.Messenger;

namespace Xioru.Messaging.Messenger
{
    public partial class MessengerGrain //TODO: move to handler ?
    {
        public async Task OnMessage(string message, string chatId)
        {
            if (string.IsNullOrWhiteSpace(message) ||
                string.IsNullOrWhiteSpace(chatId) ||
                _config == null)
            {
                return;
            }

            using var reader = new StringReader(message);
            do
            {
                var line = await reader.ReadLineAsync();

                if (line == null)
                {
                    break;
                }

                // skip comment
                if (line.StartsWith('#'))
                {
                    continue;
                }

                if (!line.StartsWith('/'))
                {
                    await SendDirectMessage(chatId, "Bad syntax. Abort.");
                    break;
                }

                var commandText = line.TrimStart('/');
                var commandName = Regex.Match(commandText, @"^([\w\-]+)").Value;

                // first, try find messenger command
                if (_commands.TryGetValue(commandName, out var command))
                {
                    var context = new MessengerCommandContext()
                    {
                        IsSupervisor = _config.Supervisors?.Any(x => x == chatId) == true,
                        ChatId = chatId,
                        Manager = _repository,
                        MessengerType = MessengerType,
                        CommandText = commandText
                    };

                    var messengerCommandResult = await command.Execute(context);
                    await SendDirectMessage(chatId, messengerCommandResult.Message);

                    if (!messengerCommandResult.IsSuccess)
                    {
                        break;
                    }

                    continue;
                }

                // else, send to channel channel
                if (_repository.TryGetCurrentChannel(chatId, out var channel))
                {
                    var stream = GetChannelStream(channel.ChannelId);
                    await stream.OnNextAsync(new ChannelIncomingMessage
                    {
                        IsSupervisor = _config.Supervisors?.Any(x => x == chatId) == true,
                        Created = DateTime.UtcNow,
                        Text = line
                    });

                    continue;
                }
                else
                {
                    await SendDirectMessage(chatId, "Project not selected. Abort.");
                    break;
                }
            }
            while (true);
        }
    }
}
