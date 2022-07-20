using Xioru.Messaging.Contracts.Channel;
using Xioru.Messaging.Contracts.Formatting;
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

            var resultText = new FormattedString();

            using var reader = new StringReader(message);
            var line = await reader.ReadLineAsync();
            while (line != null)
            {
                var segments = line.Split(" ");

                if (segments == null ||
                    segments.Length <= 0 ||
                    !segments![0].StartsWith('/'))
                {
                    return;
                }

                var context = new MessengerCommandContext()
                {
                    IsSupervisor = _config.Supervisors.Any(x => x == chatId),
                    ChatId = chatId,
                    Manager = _repository,
                    MessengerType = MessengerType
                };

                var commandText = segments[0].TrimStart('/');

                // find messenger command or send to channel
                if (segments.Length >= 2 &&
                    _commands.TryGetValue($"{commandText}.{segments[1].ToLower()}", out var command))
                {
                    context.Arguments = segments.Skip(2).ToArray();
                }
                else if (_commands.TryGetValue($"{commandText}", out command))
                {
                    context.Arguments = segments.Skip(1).ToArray();
                }
                else
                {
                    if (_repository.TryGetCurrentChannel(chatId, out var channel))
                    {
                        var stream = GetChannelStream(channel.ChannelId);
                        await stream.OnNextAsync(new ChannelIncomingMessage
                        {
                            IsSupervisor = _config.Supervisors.Any(x => x == chatId),
                            Created = DateTime.UtcNow,
                            Text = line
                        });

                        // go next line
                        line = await reader.ReadLineAsync();
                        continue;
                    }
                    else
                    {
                        await SendDirectMessage(chatId, "Project not selected. Abort.");
                        return;
                    }
                }

                if (command == null)
                {
                    await SendDirectMessage(chatId, "Internal error. Abort.");
                    return;
                }

                var result = await command.Execute(context);
                resultText.Append(result.Message); // + Environment.NewLine; //to discord grain?

                // abort if not successed
                if (!result.IsSuccess)
                {
                    await SendDirectMessage(chatId, resultText);
                    return;
                }

                line = await reader.ReadLineAsync();
            }//while lines

            if (!string.IsNullOrWhiteSpace(resultText))
            {
                await SendDirectMessage(chatId, resultText);
            }
        }
    }
}
