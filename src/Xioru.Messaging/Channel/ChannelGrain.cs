using Orleans;
using Orleans.Runtime;
using Orleans.Streams;
using Orleans.Streams.Core;
using Xioru.Grain.AbstractGrain;
using Xioru.Grain.Contracts;
using Xioru.Messaging.Contracts;
using Xioru.Messaging.Contracts.Channel;

namespace Xioru.Messaging.Channel
{
    [ImplicitStreamSubscription(MessagingConstants.ChannelIncomingStreamNamespace)]
    public partial class ChannelGrain : AbstractGrain<
        ChannelGrain,
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
            State.MessengerId = createCommand.MessengerId;
            State.ChatId = createCommand.ChatId;
            
            return Task.CompletedTask;
        }

        protected override async Task OnCreateEmitEvent(CreateChannelCommand createCommand)
        {
            await EmitEvent(GrainMessage.MessageKind.Create, new ChannelCreatedEvent(
                DisplayName: State.DisplayName,
                Description: State.Description,
                Tags: State.Tags.ToArray(),
                MessengerType: State.MessengerType,
                MessengerId: State.MessengerId,
                ChatId: State.ChatId));
        }

        protected override Task OnCreated() => Task.CompletedTask;

        protected override async Task OnDeleteEmitEvent()
        {
            await EmitEvent(GrainMessage.MessageKind.Delete);
        }

        protected override Task OnDeleted() => Task.CompletedTask;

        protected override Task OnUpdateApplyState(
            UpdateChannelCommand updateCommand) => Task.CompletedTask;

        protected override async Task OnUpdateEmitEvent(UpdateChannelCommand updateCommand)
        {
            await EmitEvent(GrainMessage.MessageKind.Create, new ChannelUpdatedEvent(
                DisplayName: updateCommand.DisplayName,
                Description: updateCommand.Description,
                Tags: updateCommand.Tags.ToArray()));
        }

        protected override Task OnUpdated() => Task.CompletedTask;

        public async Task SendMessage(string textMessage)
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
                streamId: State.MessengerId,
                streamNamespace: MessagingConstants.ChannelOutcomingStreamNamespace);

            return outcomingStream;
        }
    }
}
