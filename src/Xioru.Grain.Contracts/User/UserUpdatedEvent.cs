using Xioru.Grain.Contracts.AbstractGrain;

namespace Xioru.Grain.Contracts.User
{
    public record UserUpdatedEvent(
        string DisplayName,
        string Description,
        string[] Tags) : AbstractGrainUpdatedEvent(
            DisplayName,
            Description,
            Tags);
}
