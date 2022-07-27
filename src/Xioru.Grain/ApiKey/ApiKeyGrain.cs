using Microsoft.Extensions.Logging;
using Orleans.Runtime;
using Xioru.Grain.AbstractGrain;
using Xioru.Grain.Contracts;
using Xioru.Grain.Contracts.ApiKey;
using Xioru.Grain.Contracts.ApiKey.Events;

namespace Xioru.Grain.ApiKey
{
    public class ApiKeyGrain : AbstractGrain<
        ApiKeyState,
        CreateApiKeyCommand,
        UpdateApiKeyCommand,
        ApiKeyProjection>,
        IApiKeyGrain 
    {
        public ApiKeyGrain(
            [PersistentState("state", GrainConstants.StateStorageName)] IPersistentState<ApiKeyState> state,
            ILoggerFactory loggerFactory,
            IServiceProvider services) : base(state, loggerFactory, services)
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
            await EmitEvent(new ApiKeyCreatedEvent(
                DisplayName: State.DisplayName,
                Description: State.Description,
                Tags: State.Tags.ToArray(),
                Created: State.Created,
                ApiKey: State.ApiKey));
        }

        protected override Task OnCreated() => Task.CompletedTask;

        protected override async Task EmitDeleteEvent()
        {
            await EmitEvent(new ApiKeyDeletedEvent());
        }

        protected override Task OnUpdateApplyState(UpdateApiKeyCommand updateCommand)
        {
            return Task.CompletedTask;
        }

        protected override async Task OnUpdateEmitEvent(UpdateApiKeyCommand updateCommand)
        {
            await EmitEvent(new ApiKeyUpdatedEvent(
                DisplayName: updateCommand.DisplayName,
                Description: updateCommand.Description,
                Tags: updateCommand.Tags.ToArray()));
        }

        protected override Task OnUpdated() => Task.CompletedTask;
    }
}
