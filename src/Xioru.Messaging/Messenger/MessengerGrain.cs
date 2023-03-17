using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Orleans.Runtime;
using Orleans.Streams;
using Xioru.Grain;
using Xioru.Grain.Contracts;
using Xioru.Grain.Contracts.Messages;
using Xioru.Grain.Contracts.Project.Events;
using Xioru.Messaging.Contracts;
using Xioru.Messaging.Contracts.Channel;
using Xioru.Messaging.Contracts.Channel.Events;
using Xioru.Messaging.Contracts.Config;
using Xioru.Messaging.Contracts.Formatting;
using Xioru.Messaging.Contracts.Messenger;

namespace Xioru.Messaging.Messenger;

public abstract partial class MessengerGrain :
    Orleans.Grain,
    IMessengerGrain,
    IRemindable,
    IAsyncObserver<ChannelOutcomingMessage>,
    IAsyncObserver<GrainEvent>
{
    protected readonly ILogger<MessengerGrain> _logger;
    protected readonly IGrainFactory _grainFactory;
    protected readonly IMessengerRepository _repository;

    protected MessengerSection? _config = default!;
    private readonly Dictionary<string, IMessengerCommand> _commands;

    private IAsyncStream<GrainEvent> _clusterRepositoryStream = default!;
    private IAsyncStream<ChannelOutcomingMessage> _channelOutcomingStream = default!;

    protected abstract MessengerType MessengerType { get; }

    public MessengerGrain(
        ILogger<MessengerGrain> logger,
        IGrainFactory grainFactory,
        IMessengerRepository repository,
        IEnumerable<IMessengerCommand> commands,
        IOptions<BotsSection> options)
    {
        _logger = logger;
        _grainFactory = grainFactory;
        _repository = repository;

        if (!options.Value.Configs.TryGetValue(this.MessengerType, out _config))
        {
            _logger.LogError($"Configuration for {MessengerType.ToString()} messenger type not found");
        }

        _commands = commands.ToDictionary(x => x.Name);
    }

    public virtual async Task StartAsync()
    {
        try
        {
            // init repository
            await _repository.StartAsync(this.MessengerType);

            // init subscriptions
            var streamProvider = this.GetStreamProvider(GrainConstants.StreamProviderName);

            _clusterRepositoryStream = await streamProvider
                .GetStreamAndSingleSubscribe<GrainEvent>(
                    streamId: GrainConstants.ClusterStreamId,
                    streamNamespace: GrainConstants.ClusterRepositoryStreamNamespace,
                    observer: this);

            _channelOutcomingStream = await streamProvider
                .GetStreamAndSingleSubscribe<ChannelOutcomingMessage>(
                    streamId: Guid.Empty,
                    streamNamespace: MessagingConstants.GetChannelOutcomingStreamNamespace(this.MessengerType),
                    observer: this);

            // prevent from sleep
            await this.RegisterOrUpdateReminder(
                "KeepAlive",
                TimeSpan.FromMinutes(15),
                TimeSpan.FromMinutes(15));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error during activate messenger grain for '{MessengerType}'");
        }
    }

    public Task OnCompletedAsync()
    {
        _logger.LogDebug($"{MessengerType} completed");
        return Task.CompletedTask;
    }

    public Task OnErrorAsync(Exception ex)
    {
        _logger.LogError(ex, "Error at consume stream in MessengerGrain");
        return Task.CompletedTask;
    }

    public async Task OnNextAsync(
        ChannelOutcomingMessage item,
        StreamSequenceToken? token)
    {
        await SendDirectMessage(item.ChatId, item.Message);
        
    }

    public async Task OnNextAsync(
        GrainEvent grainEvent,
        StreamSequenceToken? token)
    {
        switch (grainEvent)
        {
            case ProjectDeletedEvent:
                await _repository.OnProjectDeleted(grainEvent.Metadata!.GrainId);
                break;

            case ChannelDeletedEvent:
                await _repository.OnChannelDeleted(grainEvent.Metadata!.GrainId);
                break;
        }
    }

    private IAsyncStream<ChannelIncomingMessage> GetChannelStream(Guid channelId)
    {
        var streamProvider = this.GetStreamProvider(GrainConstants.StreamProviderName);

        return streamProvider.GetStream<ChannelIncomingMessage>(StreamId.Create(
            ns: MessagingConstants.ChannelIncomingStreamNamespace,
            key: channelId));
    }

    public Task ReceiveReminder(string reminderName, TickStatus status) => Task.CompletedTask;

    protected abstract Task SendDirectMessage(string chatId, FormattedString message);
}
