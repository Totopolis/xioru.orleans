using Orleans.Runtime;
using Xioru.Grain.AbstractGrain;
using Xioru.Grain.Contracts;
using Xioru.Grain.Contracts.User;
using Xioru.Grain.Contracts.User.Events;

namespace Xioru.Grain.User;

public class UserGrain : AbstractGrain<
    UserState,
    CreateUserCommandModel,
    UpdateUserCommandModel,
    UserProjection>,
    IUserGrain
{
    public UserGrain(
        [PersistentState("state", GrainConstants.StateStorageName)] IPersistentState<UserState> state,
        IServiceProvider services) : base(state, services)
    {
    }

    protected override async Task OnCreateEmitEvent(CreateUserCommandModel createCommand)
    {
        await EmitEvent(new UserCreatedEvent(
            DisplayName: State.DisplayName,
            Description: State.Description,
            Tags: State.Tags.ToArray(),
            Login: State.Login));
    }

    protected override async Task EmitDeleteEvent()
    {
        await EmitEvent(new UserDeletedEvent());
    }

    protected override async Task OnUpdateEmitEvent(UpdateUserCommandModel updateCommand)
    {
        await EmitEvent(new UserUpdatedEvent(
            DisplayName: updateCommand.DisplayName,
            Description: updateCommand.Description,
            Tags: updateCommand.Tags.ToArray()));
    }
}
