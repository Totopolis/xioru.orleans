using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Orleans;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Xioru.Messaging.Contracts.Config;
using Xioru.Messaging.Contracts.Formatting;
using Xioru.Messaging.Contracts.Messenger;

namespace Xioru.Messaging.Messenger;

public class TelegramMessengerGrain : MessengerGrain, ITelegramMessengerGrain
{
    private TelegramBotClient? _telegramClient = default!;
    private CancellationTokenSource _cts = new();

    public TelegramMessengerGrain(
        ILogger<MessengerGrain> logger,
        IGrainFactory grainFactory,
        IMessengerRepository repository,
        IEnumerable<IMessengerCommand> commands,
        IOptions<BotsSection> options) : base(logger, grainFactory, repository, commands, options)
    {
    }

    protected override MessengerType MessengerType => MessengerType.Telegram;

    public override async Task OnDeactivateAsync(DeactivationReason reason, CancellationToken token)
    {
        _cts.Cancel();

        // TODO: increase deactivation time
        await base.OnDeactivateAsync(reason, token);
    }

    public override async Task StartAsync()
    {
        if (_config == null ||
            _telegramClient != null)
        {
            // TODO: need exception, logging?
            return;
        }

        _telegramClient = new TelegramBotClient(_config.Token);

        var receiverOptions = new ReceiverOptions()
        {
            AllowedUpdates = Array.Empty<UpdateType>(),
            ThrowPendingUpdates = true,
        };

        _telegramClient.StartReceiving(
            updateHandler: HandleUpdateAsync,
            pollingErrorHandler: HandlePollingError,
            receiverOptions: receiverOptions,
            cancellationToken: _cts.Token);

        // subscribe to cluster and channelOutcoming streams
        await base.StartAsync();

        var self = await _telegramClient.GetMeAsync();
        _logger.LogInformation($"Start listening for @{self.Username}");
    }

    protected override async Task SendDirectMessage(string chatId, FormattedString message)
    {
        if (_telegramClient == null)
        {
            _logger.LogError("Telegram client are not initialized. Failed attempt to send message\n{Message}\n", message);
            return;
        }

        var internalId = long.TryParse(chatId, out var num) ? new ChatId(num) : new ChatId("@" + chatId);
        var formattedMessage = message.ToString(
            replaces: _telegramSpecificReplaces,
            boxedLineFormatter: blstr => $"*{blstr}*\n\n",
            boldFormatter: bstr => $"*{bstr}*",
            italicFormatter: istr => $"_{istr}_",
            codeFormatter: cstr => $"```\n{cstr}```",
            limit: 4000);

        try
        {
            await _telegramClient.SendTextMessageAsync(internalId, formattedMessage, ParseMode.MarkdownV2);
        }
        catch
        {
            await _telegramClient.SendTextMessageAsync(internalId, formattedMessage);
        }            
    }

    private Task HandlePollingError(
        ITelegramBotClient botClient,
        Exception exception,
        CancellationToken _)
    {
        var errorMessage = exception switch
        {
            ApiRequestException apiRequestException 
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        _logger.LogError(errorMessage);
        return Task.CompletedTask;
    }

    private async Task HandleUpdateAsync(
        ITelegramBotClient botClient,
        Update update,
        CancellationToken _)
    {
        if (update.Type != UpdateType.Message)
        {
            _logger.LogDebug($"Telegram update of type {update.Type} is not handled");
            return;
        }

        if (update.Message == null || update.Message.Text == null)
        {
            _logger.LogError($"Telegram update message is empty");
            return;
        }

        await OnMessage(
            message: update.Message.Text,
            chatId: update.Message.Chat.Id.ToString(),
            userName: update.ChatMember?.From.Username ?? string.Empty);
    }

    private readonly Dictionary<string, string> _telegramSpecificReplaces = new Dictionary<string, string> {
        {"_", @"\_"}, {"*", @"\*"}, {"[", @"\["}, {"]", @"\]"}, {"{", @"\{"}, {"}", @"\}"}, 
        {"~", @"\~"}, {"`", @"\`"}, {">", @"\>"}, {"#", @"\#"}, {"+", @"\+"}, {"-", @"\-"},
        {"=", @"\="}, {"|", @"\|"}, {".", @"\."}, {"!", @"\!"}, {"(", @"\("}, {")", @"\)"}
    };
}
