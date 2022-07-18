using Discord;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
    public abstract partial class MessengerGrain :
        Orleans.Grain,
        IMessengerGrain,
        IRemindable,
        IAsyncObserver<ChannelOutcomingMessage>,
        IAsyncObserver<GrainMessage>
    {
        protected readonly ILogger<MessengerGrain> _log;
        protected readonly IGrainFactory _grainFactory;
        protected readonly IMessengerRepository _repository;

        protected MessengerSection? _config = null;
        private readonly Dictionary<string, IMessengerCommand> _commands;

        private IAsyncStream<GrainMessage> _clusterRepositoryStream = default!;
        private IAsyncStream<ChannelOutcomingMessage> _channelOutcomingStream = default!;

        protected abstract MessengerType MessengerType { get; }

        public MessengerGrain(
            ILogger<MessengerGrain> log,
            IGrainFactory grainFactory,
            IMessengerRepository repository,
            IEnumerable<IMessengerCommand> commands,
            IOptions<BotsConfigSection> config)
        {
            _log = log;
            _grainFactory = grainFactory;
            _repository = repository;
            _config = config.Value.Configs[this.MessengerType];

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

        public Task ReceiveReminder(string reminderName, TickStatus status)
        {
            return Task.CompletedTask;
        }

        public abstract Task StartAsync();

        protected abstract Task SendDirectMessage(string chatId, string message);
    }
}
