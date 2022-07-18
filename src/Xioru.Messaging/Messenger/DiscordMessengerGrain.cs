using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Orleans;
using Xioru.Messaging.Contracts.Config;
using Xioru.Messaging.Contracts.Messenger;

namespace Xioru.Messaging.Messenger
{
    public class DiscordMessengerGrain : MessengerGrain, IDiscordMessengerGrain
    {
        private readonly DiscordSocketClient _discord = new DiscordSocketClient(); //TODO: inject?
        public DiscordMessengerGrain(
            ILogger<MessengerGrain> logger,
            IGrainFactory grainFactory,
            IMessengerRepository repository,
            IEnumerable<IMessengerCommand> commands,
            IOptions<BotsConfigSection> botsConfig) : base(logger, grainFactory, repository, commands, botsConfig)
        {

        }

        protected override MessengerType MessengerType => MessengerType.Discord;

        public override async Task OnDeactivateAsync()
        {
            await _discord.StopAsync();

            await base.OnDeactivateAsync(); //TODO: increase deactivation time
        }

        public override async Task StartAsync()
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

        protected override async Task SendDirectMessage(string chatId, string message)
        {
            if (ulong.TryParse(chatId, out var channelId))
            {
                var channel = await _discord.GetChannelAsync(channelId) as IMessageChannel;

                if (channel != null)
                {
                    await channel.SendMessageAsync(message);
                }
                else
                {
                    _log.LogWarning($"Failed attempt to send internal message to {chatId} ({message})");
                }
            }
            else
            {
                _log.LogWarning($"Failed attempt to parse {chatId} for sending a message ({message})");
            }
        }
    }
}
