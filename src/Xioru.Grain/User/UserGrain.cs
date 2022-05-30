using Orleans.Runtime;
using Xioru.Grain.AbstractGrain;
using Xioru.Grain.Contracts;
using Xioru.Grain.Contracts.User;

namespace Xioru.Grain.User
{
    public class UserGrain : AbstractGrain<
        UserGrain,
        UserState,
        CreateUserCommand,
        UpdateUserCommand,
        UserProjection>,
        IUserGrain
    {
        public UserGrain(
            [PersistentState("state", "iotStore")] IPersistentState<UserState> state,
            IServiceProvider services) : base(state, services)
        {
        }

        protected override Task OnCreateApplyState(CreateUserCommand createCommand)
        {
            State.Login = createCommand.Login;

            return Task.CompletedTask;
        }

        protected override async Task OnCreateEmitEvent(CreateUserCommand createCommand)
        {
            await EmitEvent(GrainMessage.MessageKind.Create, new UserCreatedEvent(
                DisplayName: State.DisplayName,
                Description: State.Description,
                Tags: State.Tags.ToArray(),
                Login: State.Login));
        }

        protected override async Task OnDeleteEmitEvent()
        {
            await EmitEvent(GrainMessage.MessageKind.Delete);
        }

        protected override Task OnUpdateApplyState(UpdateUserCommand updateCommand)
        {
            return Task.CompletedTask;
        }

        protected override async Task OnUpdateEmitEvent(UpdateUserCommand updateCommand)
        {
            await EmitEvent(GrainMessage.MessageKind.Create, new UserUpdatedEvent(
                DisplayName: updateCommand.DisplayName,
                Description: updateCommand.Description,
                Tags: updateCommand.Tags.ToArray()));
        }
    }
}
