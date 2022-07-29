using Xioru.Grain.Contracts.AbstractGrain;

namespace Xioru.Grain.Contracts.User.Events
{
    public record UserUpdatedEvent(
        string DisplayName,
        string Description,
        string[] Tags) : AbstractGrainUpdatedEvent(
            DisplayName,
            Description,
            Tags);
}
