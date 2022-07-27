using Microsoft.Extensions.Logging;
using Orleans.Runtime;
using Xioru.Grain.AbstractGrain;
using Xioru.Grain.Contracts;
using Xioru.Grain.Contracts.Messages;
using Xioru.Grain.Contracts.User;
using Xioru.Grain.Contracts.User.Events;

namespace Xioru.Grain.User
{
    public class UserGrain : AbstractGrain<
        UserState,
        CreateUserCommand,
        UpdateUserCommand,
        UserProjection>,
        IUserGrain
    {
        public UserGrain(
            [PersistentState("state", GrainConstants.StateStorageName)] IPersistentState<UserState> state,
            ILoggerFactory loggerFactory,
            IServiceProvider services) : base(state, loggerFactory, services)
        {
        }

        protected override Task OnCreateApplyState(CreateUserCommand createCommand)
        {
            State.Login = createCommand.Login;

            return Task.CompletedTask;
        }

        protected override async Task OnCreateEmitEvent(CreateUserCommand createCommand)
        {
            await EmitEvent(new UserCreatedEvent(
                DisplayName: State.DisplayName,
                Description: State.Description,
                Tags: State.Tags.ToArray(),
                Login: State.Login));
        }

        protected override Task OnCreated() => Task.CompletedTask;

        protected override async Task EmitDeleteEvent()
        {
            await EmitEvent(new UserDeletedEvent());
        }

        protected override Task OnUpdateApplyState(UpdateUserCommand updateCommand)
        {
            return Task.CompletedTask;
        }

        protected override async Task OnUpdateEmitEvent(UpdateUserCommand updateCommand)
        {
            await EmitEvent(new UserUpdatedEvent(
                DisplayName: updateCommand.DisplayName,
                Description: updateCommand.Description,
                Tags: updateCommand.Tags.ToArray()));
        }

        protected override Task OnUpdated() => Task.CompletedTask;
    }
}
