using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace Xioru.Messaging.Messenger
{
    public partial class MessengerGrain
    {
        private readonly DiscordSocketClient _discord = new DiscordSocketClient();

        private async Task StartDiscord()
        {
            _discord.Log += LogAsync;
            _discord.Ready += ReadyAsync;
            _discord.MessageReceived += MessageReceivedAsync;

            await _discord.LoginAsync(TokenType.Bot, _config!.Token);
            await _discord.StartAsync();
        }

        private Task LogAsync(LogMessage log)
        {
            _log.LogInformation(log.ToString());
            return Task.CompletedTask;
        }

        // The Ready event indicates that the client has opened a
        // connection and it is now safe to access the cache.
        private Task ReadyAsync()
        {
            _log.LogInformation($"{_discord.CurrentUser} is connected!");
            return Task.CompletedTask;
        }

        // This is not the recommended way to write a bot - consider
        // reading over the Commands Framework sample.
        private async Task MessageReceivedAsync(SocketMessage message)
        {
            // The bot should never respond to itself.
            if (message.Author.Id == _discord.CurrentUser.Id)
            {
                return;
            }

            await OnMessage(
                message: message.Content,
                chatId: message.Channel.Id.ToString());
        }
    }
}
