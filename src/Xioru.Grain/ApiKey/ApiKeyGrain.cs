using Orleans.Runtime;
using Xioru.Grain.AbstractGrain;
using Xioru.Grain.Contracts;
using Xioru.Grain.Contracts.ApiKey;

namespace Xioru.Grain.ApiKey
{
    public class ApiKeyGrain : AbstractGrain<
        ApiKeyGrain,
        ApiKeyState,
        CreateApiKeyCommand,
        UpdateApiKeyCommand,
        ApiKeyProjection>,
        IApiKeyGrain 
    {
        public ApiKeyGrain(
            [PersistentState("state", GrainConstants.StateStorageName)] IPersistentState<ApiKeyState> state,
            IServiceProvider services) : base(state, services)
        {
        }

        protected override Task OnCreateApplyState(CreateApiKeyCommand createCommand)
        {
            State.Created = DateTime.UtcNow;
            State.ApiKey = Guid.NewGuid();

            return Task.CompletedTask;
        }

        protected override async Task OnCreateEmitEvent(CreateApiKeyCommand createCommand)
        {
            await EmitEvent(GrainMessage.MessageKind.Create, new ApiKeyCreatedEvent(
                DisplayName: State.DisplayName,
                Description: State.Description,
                Tags: State.Tags.ToArray(),
                Created: State.Created,
                ApiKey: State.ApiKey));
        }

        protected override async Task OnDeleteEmitEvent()
        {
            await EmitEvent(GrainMessage.MessageKind.Delete);
        }

        protected override Task OnUpdateApplyState(UpdateApiKeyCommand updateCommand)
        {
            return Task.CompletedTask;
        }

        protected override async Task OnUpdateEmitEvent(UpdateApiKeyCommand updateCommand)
        {
            await EmitEvent(GrainMessage.MessageKind.Create, new ApiKeyUpdatedEvent(
                DisplayName: updateCommand.DisplayName,
                Description: updateCommand.Description,
                Tags: updateCommand.Tags.ToArray()));
        }
    }
}
