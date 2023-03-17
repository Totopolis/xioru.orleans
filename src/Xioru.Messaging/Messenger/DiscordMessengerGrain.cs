using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Xioru.Messaging.Contracts.Config;
using Xioru.Messaging.Contracts.Formatting;
using Xioru.Messaging.Contracts.Messenger;

namespace Xioru.Messaging.Messenger;

public class DiscordMessengerGrain : MessengerGrain, IDiscordMessengerGrain
{
    private readonly DiscordSocketClient _discordClient;

    public DiscordMessengerGrain(
        ILogger<MessengerGrain> logger,
        IGrainFactory grainFactory,
        IMessengerRepository repository,
        IEnumerable<IMessengerCommand> commands,
        IOptions<BotsSection> options) : base(logger, grainFactory, repository, commands, options)
    {
        _discordClient = new DiscordSocketClient();
    }

    protected override MessengerType MessengerType => MessengerType.Discord;

    public override async Task OnDeactivateAsync(DeactivationReason reason, CancellationToken token)
    {
        await _discordClient.StopAsync();

        // TODO: increase deactivation time
        await base.OnDeactivateAsync(reason, token);
    }

    public override async Task StartAsync()
    {
        if (_config == null ||
            _discordClient.LoginState == LoginState.LoggedIn)
        {
            return;
        }

        _discordClient.Log += LogAsync;
        _discordClient.Ready += ReadyAsync;
        _discordClient.MessageReceived += MessageReceivedAsync;

        await _discordClient.LoginAsync(TokenType.Bot, _config!.Token);
        await _discordClient.StartAsync();

        // subscribe to cluster and channelOutcoming streams
        await base.StartAsync();
    }

    private Task LogAsync(LogMessage log)
    {
        _logger.LogInformation(log.ToString());
        return Task.CompletedTask;
    }

    // The Ready event indicates that the client has opened a
    // connection and it is now safe to access the cache.
    private Task ReadyAsync()
    {
        _logger.LogInformation($"{_discordClient.CurrentUser} is connected!");
        return Task.CompletedTask;
    }

    // This is not the recommended way to write a bot - consider
    // reading over the Commands Framework sample.
    private async Task MessageReceivedAsync(SocketMessage message)
    {
        // The bot should never respond to itself.
        if (message.Author.Id == _discordClient.CurrentUser.Id)
        {
            return;
        }

        await OnMessage(
            message: message.Content,
            chatId: message.Channel.Id.ToString(),
            userName: message.Author.Username);
    }

    protected override async Task SendDirectMessage(string chatId, FormattedString message)
    {
        if (_discordClient.LoginState != LoginState.LoggedIn)
        {
            return;
        }

        if (!ulong.TryParse(chatId, out var channelId))
        {
            _logger.LogWarning($"Failed attempt to parse {chatId} for sending a message ({message})"); 
        }

        var channel = await _discordClient.GetChannelAsync(channelId) as IMessageChannel;

        if (channel == null)
        {
            _logger.LogWarning($"Failed attempt to send internal message to {chatId} ({message})");
            return;
        }

        var formattedMessage = message.ToString(
            replaces: new Dictionary<string, string>(), //TODO: remake to a lambda
            boldFormatter: bstr => $"**{bstr}**",
            boxedLineFormatter: blstr => $"> {blstr}\n",
            italicFormatter: istr => $"*{istr}*",
            codeFormatter: cstr => $"```{cstr}```",
            limit: 2000);

        await channel.SendMessageAsync(formattedMessage);
    }
}
