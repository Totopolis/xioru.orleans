using Xioru.Grain.Contracts.AbstractGrain;

namespace Xioru.Grain.Contracts.User.Events
{
    public record UserCreatedEvent(
        string DisplayName,
        string Description,
        string[] Tags,
        //
        string Login) : AbstractGrainCreatedEvent(
            DisplayName,
            Description,
            Tags);
}
