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
        CreateFooCommand,
        UpdateFooCommand,
        FooProjection>,
        IFooGrain 
    {
        public FooGrain(
            [PersistentState("state", GrainConstants.StateStorageName)] IPersistentState<FooState> state,
            IServiceProvider services) : base(state, services)
        {
        }

        protected override Task OnCreateApplyState(CreateFooCommand createCommand)
        {
            State.FooData = createCommand.FooData;

            return Task.CompletedTask;
        }

        protected override async Task OnCreateEmitEvent(CreateFooCommand createCommand)
        {
            await EmitEvent(new FooCreatedEvent(
                DisplayName: State.DisplayName,
                Description: State.Description,
                Tags: State.Tags.ToArray(),
                FooData: "Hello World!"));
        }

        protected override Task OnCreated() => Task.CompletedTask;

        protected override async Task EmitDeleteEvent()
        {
            await EmitEvent(new FooDeletedEvent());
        }

        protected override Task OnUpdateApplyState(UpdateFooCommand updateCommand)
        {
            return Task.CompletedTask;
        }

        protected override async Task OnUpdateEmitEvent(UpdateFooCommand updateCommand)
        {
            await EmitEvent(new FooUpdatedEvent(
                DisplayName: updateCommand.DisplayName,
                Description: updateCommand.Description,
                Tags: updateCommand.Tags.ToArray(),
                FooData: "Bye World!"));
        }

        protected override Task OnUpdated() => Task.CompletedTask;
    }
}
