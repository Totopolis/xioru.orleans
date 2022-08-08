using Orleans.Runtime;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xioru.Grain.AbstractGrain;
using Xioru.Grain.Contracts;

namespace Xioru.Orleans.Tests.Foo
{
    public class FooGrain : AbstractGrain<
        FooState,
        CreateFooCommandModel,
        UpdateFooCommandModel,
        FooProjection>,
        IFooGrain 
    {
        public FooGrain(
            [PersistentState("state", GrainConstants.StateStorageName)] IPersistentState<FooState> state,
            IServiceProvider services) : base(state, services)
        {
        }

        public override async Task CreateAsync(CreateFooCommandModel createCommand)
        {
            State.FooData = createCommand.FooData;
            await base.CreateAsync(createCommand);
        }

        protected override async Task OnCreateEmitEvent(CreateFooCommandModel createCommand)
        {
            await EmitEvent(new FooCreatedEvent(
                DisplayName: State.DisplayName,
                Description: State.Description,
                Tags: State.Tags.ToArray(),
                FooData: "Hello World!"));
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
                FooData: "Bye World!"));
        }
    }
}
