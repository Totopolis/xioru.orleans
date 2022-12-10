using Orleans.Runtime;
using Orleans.Streams;
using Orleans.Streams.Core;
using Xioru.Grain.AbstractGrain;
using Xioru.Grain.Contracts;
using Xioru.Messaging.Contracts;
using Xioru.Messaging.Contracts.Channel;
using Xioru.Messaging.Contracts.Channel.Events;
using Xioru.Messaging.Contracts.Formatting;

namespace Xioru.Messaging.Channel;

[ImplicitStreamSubscription(MessagingConstants.ChannelIncomingStreamNamespace)]
public partial class ChannelGrain : AbstractGrain<
    ChannelState,
    CreateChannelCommandModel,
    UpdateChannelCommandModel,
    ChannelProjection>,
    IChannelGrain,
    IStreamSubscriptionObserver,
    IAsyncBatchObserver<ChannelIncomingMessage>
{
    private Lazy<IAsyncStream<ChannelOutcomingMessage>> _outcomingStream;

    public ChannelGrain(
        [PersistentState("state", GrainConstants.StateStorageName)] IPersistentState<ChannelState> state,
        IServiceProvider services) : base(state, services)
    {
        _outcomingStream = new Lazy<IAsyncStream<ChannelOutcomingMessage>>(
            GetLazyOutcomingStream);
    }

    protected override async Task OnCreateEmitEvent(CreateChannelCommandModel createCommand)
    {
        await EmitEvent(new ChannelCreatedEvent(
            DisplayName: State.DisplayName,
            Description: State.Description,
            Tags: State.Tags.ToArray(),
            CreatedUtc: State.CreatedUtc,
            MessengerType: State.MessengerType,
            ChatId: State.ChatId));
    }

    protected override async Task EmitDeleteEvent()
    {
        await EmitEvent(new ChannelDeletedEvent());
    }

    protected override async Task OnUpdateEmitEvent(UpdateChannelCommandModel updateCommand)
    {
        await EmitEvent(new ChannelUpdatedEvent(
            DisplayName: updateCommand.DisplayName,
            Description: updateCommand.Description,
            Tags: updateCommand.Tags.ToArray(),
            UpdatedUtc: State.UpdatedUtc));
    }

    public async Task SendMessage(FormattedString textMessage)
    {
        await _outcomingStream.Value
            .OnNextAsync(new ChannelOutcomingMessage
            {
                ChatId = State.ChatId,
                Message = textMessage
            });
    }

    private IAsyncStream<ChannelOutcomingMessage> GetLazyOutcomingStream()
    {
        var _streamProvider = this.GetStreamProvider(GrainConstants.StreamProviderName);
        var outcomingStream = _streamProvider.GetStream<ChannelOutcomingMessage>(StreamId.Create(
            ns: MessagingConstants.GetChannelOutcomingStreamNamespace(State.MessengerType),
            key: Guid.Empty));

        return outcomingStream;
    }
}
