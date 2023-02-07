using Orleans.Runtime;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xioru.Grain.AbstractGrain;
using Xioru.Grain.Contracts;
using Xioru.Orleans.Tests.Contracts;

namespace Xioru.Orleans.Tests.Domain;

public class FooGrain : AbstractGrain<
    FooState,
    CreateFooCommandModel,
    UpdateFooCommandModel,
    FooProjection>,
    IFooGrain 
{
    public FooGrain(
        [PersistentState("State", GrainConstants.StateStorageName)] IPersistentState<FooState> state,
        IServiceProvider services) : base(state, services)
    {
    }

    protected override async Task OnCreateEmitEvent(CreateFooCommandModel createCommand)
    {
        await EmitEvent(new FooCreatedEvent(
            DisplayName: State.DisplayName,
            Description: State.Description,
            Tags: State.Tags.ToArray(),
            State.CreatedUtc,
            FooData: State.FooData,
            FooMeta: State.FooMeta));
    }

    protected override async Task EmitDeleteEvent()
    {
        await EmitEvent(new FooDeletedEvent());
    }

    protected override async Task OnUpdateEmitEvent(UpdateFooCommandModel updateCommand)
    {
        await EmitEvent(new FooUpdatedEvent(
            DisplayName: updateCommand.DisplayName,
            Description: updateCommand.Description,
            Tags: updateCommand.Tags.ToArray(),
            UpdatedUtc: State.UpdatedUtc,
            FooData: State.FooData,
            FooMeta: State.FooMeta));
    }
}
