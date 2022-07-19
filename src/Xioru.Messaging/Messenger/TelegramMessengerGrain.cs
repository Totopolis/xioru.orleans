using Telegram.Bot;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Orleans;
using Xioru.Messaging.Contracts.Config;
using Xioru.Messaging.Contracts.Messenger;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Polling;
using Telegram.Bot.Exceptions;
using Xioru.Grain.Contracts;

namespace Xioru.Messaging.Messenger
{

    [ImplicitStreamSubscription(GrainConstants.ClusterRepositoryStreamNamespace)]
    public class TelegramMessengerGrain : MessengerGrain, ITelegramMessengerGrain
    {
        private readonly ITelegramBotClient _telegramClient;
        private CancellationTokenSource _cts = new CancellationTokenSource();

        public TelegramMessengerGrain(
            ILogger<MessengerGrain> logger,
            IGrainFactory grainFactory,
            IMessengerRepository repository,
            IEnumerable<IMessengerCommand> commands,
            IOptions<BotsConfigSection> botsConfig,
            ITelegramBotClient telegramBotClient) : base(logger, grainFactory, repository, commands, botsConfig)
        {
            _telegramClient = telegramBotClient;
        }

        protected override MessengerType MessengerType => MessengerType.Telegram;

        public override async Task OnDeactivateAsync()
        {
            _cts.Cancel();
            await base.OnDeactivateAsync(); //TODO: increase deactivation time
        }

        public override async Task StartAsync()
        {
            var receiverOptions = new ReceiverOptions()
            {
                AllowedUpdates = Array.Empty<UpdateType>(),
                ThrowPendingUpdates = true,
            };

            _telegramClient.StartReceiving(updateHandler: HandleUpdateAsync,
                               pollingErrorHandler: HandlePollingError,
                               receiverOptions: receiverOptions,
                               cancellationToken: _cts.Token);

            var self = await _telegramClient.GetMeAsync();
            _logger.LogInformation($"Start listening for @{self.Username}");
        }

        protected override async Task SendDirectMessage(string chatId, string message)
        {
            var internalId = long.TryParse(chatId, out var num) ? new ChatId(num) : new ChatId("@" + chatId);
            await _telegramClient.SendTextMessageAsync(internalId, message);
            //TODO: checks needed?
        }

        private Task HandlePollingError(ITelegramBotClient botClient, Exception exception, CancellationToken _)
        {
            var errorMessage = exception switch
            {
                ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            _logger.LogError(errorMessage);
            return Task.CompletedTask;
        }

        async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken _)
        {
            if (update.Type != UpdateType.Message)
            {
                _logger.LogDebug($"Telegram update of type {update.Type} is not handled");
                return;
            }

            if (update.Message == null || update.Message.Text == null)
            {
                _logger.LogError($"Telegram updatemessage is empty");
                return;
            }

            await OnMessage(update.Message.Text, update.Message.Chat.Id.ToString());
        }
    }
}
