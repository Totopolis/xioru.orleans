using Orleans.Runtime;
using Xioru.Grain.AbstractGrain;
using Xioru.Grain.Contracts;
using Xioru.Grain.Contracts.ApiKey;
using Xioru.Grain.Contracts.ApiKey.Events;

namespace Xioru.Grain.ApiKey;

public class ApiKeyGrain : AbstractGrain<
    ApiKeyState,
    CreateApiKeyCommandModel,
    UpdateApiKeyCommandModel,
    ApiKeyProjection>,
    IApiKeyGrain 
{
    public ApiKeyGrain(
        [PersistentState("State", GrainConstants.StateStorageName)] IPersistentState<ApiKeyState> state,
        IServiceProvider services) : base(state, services)
    {
    }

    protected override async Task OnCreateEmitEvent(CreateApiKeyCommandModel createCommand)
    {
        await EmitEvent(new ApiKeyCreatedEvent(
            DisplayName: State.DisplayName,
            Description: State.Description,
            Tags: State.Tags.ToArray(),
            CreatedUtc: State.CreatedUtc,
            ApiKey: State.ApiKey));
    }

    protected override async Task EmitDeleteEvent()
    {
        await EmitEvent(new ApiKeyDeletedEvent());
    }

    protected override async Task OnUpdateEmitEvent(UpdateApiKeyCommandModel updateCommand)
    {
        await EmitEvent(new ApiKeyUpdatedEvent(
            DisplayName: updateCommand.DisplayName,
            Description: updateCommand.Description,
            Tags: updateCommand.Tags.ToArray(),
            UpdatedUtc: State.UpdatedUtc));
    }
}
