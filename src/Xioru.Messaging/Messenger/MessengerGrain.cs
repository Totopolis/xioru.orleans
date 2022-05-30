using Discord;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Runtime;
using Orleans.Streams;
using Orleans.Streams.Core;
using Xioru.Grain;
using Xioru.Grain.Contracts;
using Xioru.Messaging.Contracts;
using Xioru.Messaging.Contracts.Channel;
using Xioru.Messaging.Contracts.Config;
using Xioru.Messaging.Contracts.Messenger;

namespace Xioru.Messaging.Messenger
{
    [ImplicitStreamSubscription(MessagingConstants.ChannelOutcomingStreamNamespace)]
    [ImplicitStreamSubscription(GrainConstants.ClusterRepositoryStreamNamespace)]
    public partial class MessengerGrain :
        Orleans.Grain,
        IMessengerGrain,
        IRemindable,
        //IStreamSubscriptionObserver,
        IAsyncObserver<ChannelOutcomingMessage>,
        IAsyncObserver<GrainMessage>
    {
        private readonly ILogger<MessengerGrain> _log;
        private readonly IGrainFactory _grainFactory;
        private readonly IMessengerRepository _repository;

        private MessengerSection? _config = null;
        private readonly Dictionary<string, IMessengerCommand> _commands;

        private IAsyncStream<GrainMessage> _clusterRepositoryStream = default!;
        private IAsyncStream<ChannelOutcomingMessage> _channelOutcomingStream = default!;

        public MessengerGrain(
            ILogger<MessengerGrain> log,
            IGrainFactory grainFactory,
            IMessengerRepository repository,
            IEnumerable<IMessengerCommand> commands)
        {
            _log = log;
            _grainFactory = grainFactory;
            _repository = repository;

            _commands = new Dictionary<string, IMessengerCommand>();

            var array = commands.ToArray();
            array
                .Where(x => x.IsSubCommandExists)
                .ToList()
                .ForEach(x => _commands.Add($"{x.CommandName}.{x.SubCommandName}", x));

            array
                .Where(x => !x.IsSubCommandExists)
                .ToList()
                .ForEach(x => _commands.Add($"{x.CommandName}", x));
        }

        public override async Task OnActivateAsync()
        {
            var id = this.GetPrimaryKey();

            try
            {
                // init repository
                await _repository.StartAsync(id);

                // init subscriptions
                var streamProvider = this.GetStreamProvider("SMSProvider");

                _clusterRepositoryStream = await streamProvider
                    .GetStreamAndSingleSubscribe<GrainMessage>(
                        streamId: GrainConstants.ClusterStreamId,
                        streamNamespace: GrainConstants.ClusterRepositoryStreamNamespace,
                        observer: this);

                _channelOutcomingStream = await streamProvider
                    .GetStreamAndSingleSubscribe<ChannelOutcomingMessage>(
                        streamId: id,
                        streamNamespace: MessagingConstants.ChannelOutcomingStreamNamespace,
                        observer: this);

                // prevent from sleep
                await RegisterOrUpdateReminder(
                    "KeepAlive",
                    TimeSpan.FromMinutes(1),
                    TimeSpan.FromMinutes(1));
            }
            catch (Exception ex)
            {
                _log.LogError(ex, $"Error during activate messenger grain '{id}'");
            }

            await base.OnActivateAsync();
        }

        public override async Task OnDeactivateAsync()
        {
            await _discord.StopAsync();

            await base.OnDeactivateAsync();
        }

        public Task OnCompletedAsync()
        {
            return Task.CompletedTask;
        }

        public Task OnErrorAsync(Exception ex)
        {
            _log.LogError(ex, "Error at consume stream in MessengerGrain");
            return Task.CompletedTask;
        }

        public async Task OnNextAsync(
            ChannelOutcomingMessage item,
            StreamSequenceToken token)
        {
            if (_config == null)
            {
                return;
            }

            await SendDirectMessage(item.ChatId, item.Message);
        }

        public async Task OnNextAsync(
            GrainMessage item,
            StreamSequenceToken token)
        {
            if (_config == null)
            {
                return;
            }

            if (item.Kind != GrainMessage.MessageKind.Delete)
            {
                return;
            }

            if (item.GrainType == "ProjectGrain")
            {
                await _repository.OnProjectDeleted(item.GrainId);
            }
            else if (item.GrainType == "ChannelGrain")
            {
                await _repository.OnChannelDeleted(item.GrainId);
            }
        }

        private IAsyncStream<ChannelIncomingMessage> GetChannelStream(Guid channelId)
        {
            var streamProvider = this.GetStreamProvider("SMSProvider");

            return streamProvider.GetStream<ChannelIncomingMessage>(
                channelId,
                MessagingConstants.ChannelIncomingStreamNamespace);
        }

        public async Task StartAsync(MessengerSection config)
        {
            if (_config != null)
            {
                throw new Exception("Messenger already initialized");
            }

            _config = config;

            switch (_config.Type)
            {
                case MessengerType.Discord:
                    await StartDiscord();
                    break;
                case MessengerType.Telegram:
                    throw new NotImplementedException();
                case MessengerType.Slack:
                    throw new NotImplementedException();
                case MessengerType.Teams:
                    throw new NotImplementedException();
                default:
                    throw new NotImplementedException();
            }
        }

        private async Task SendDirectMessage(string chatId, string message)
        {
            // TODO: switch telega, slack, ...
            if (ulong.TryParse(chatId, out var channelId))
            {
                var channel = await _discord.GetChannelAsync(channelId) as IMessageChannel;

                if (channel != null)
                {
                    await channel.SendMessageAsync(message);
                }
            }
        }

        public Task ReceiveReminder(string reminderName, TickStatus status)
        {
            return Task.CompletedTask;
        }
    }
}
