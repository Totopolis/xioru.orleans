using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Runtime;
using Orleans.Streams;
using Orleans.Streams.Core;
using Xioru.Grain.AbstractGrain;
using Xioru.Grain.Contracts;
using Xioru.Grain.Contracts.Messages;
using Xioru.Messaging.Contracts;
using Xioru.Messaging.Contracts.Channel;
using Xioru.Messaging.Contracts.Channel.Events;
using Xioru.Messaging.Contracts.Formatting;

namespace Xioru.Messaging.Channel
{
    [ImplicitStreamSubscription(MessagingConstants.ChannelIncomingStreamNamespace)]
    public partial class ChannelGrain : AbstractGrain<
        ChannelState,
        CreateChannelCommand,
        UpdateChannelCommand,
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

        protected override Task OnCreateApplyState(CreateChannelCommand createCommand)
        {
            State.MessengerType = createCommand.MessengerType;
            State.ChatId = createCommand.ChatId;
            
            return Task.CompletedTask;
        }

        protected override async Task OnCreateEmitEvent(CreateChannelCommand createCommand)
        {
            await EmitEvent(new ChannelCreatedEvent(
                DisplayName: State.DisplayName,
                Description: State.Description,
                Tags: State.Tags.ToArray(),
                MessengerType: State.MessengerType,
                ChatId: State.ChatId));
        }

        protected override Task OnCreated() => Task.CompletedTask;

        protected override async Task EmitDeleteEvent()
        {
            await EmitEvent(new ChannelDeletedEvent());
        }

        protected override Task OnUpdateApplyState(
            UpdateChannelCommand updateCommand) => Task.CompletedTask;

        protected override async Task OnUpdateEmitEvent(UpdateChannelCommand updateCommand)
        {
            await EmitEvent(new ChannelUpdatedEvent(
                DisplayName: updateCommand.DisplayName,
                Description: updateCommand.Description,
                Tags: updateCommand.Tags.ToArray()));
        }

        protected override Task OnUpdated() => Task.CompletedTask;

        public async Task SendMessage(FormattedString textMessage)
        {
            await _outcomingStream.Value
                .OnNextAsync(new ChannelOutcomingMessage
                {
                    MessengerType = State.MessengerType, //TODO: different streams mb?
                    ChatId = State.ChatId,
                    Message = textMessage
                });
        }

        private IAsyncStream<ChannelOutcomingMessage> GetLazyOutcomingStream()
        {
            var _streamProvider = GetStreamProvider("SMSProvider");
            var outcomingStream = _streamProvider.GetStream<ChannelOutcomingMessage>(
                streamId: Guid.Empty,
                streamNamespace: MessagingConstants.GetChannelOutcomingStreamNamespace(State.MessengerType));

            return outcomingStream;
        }
    }
}
