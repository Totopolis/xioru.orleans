using Xioru.Grain.Contracts.AbstractGrain;

namespace Xioru.Grain.Contracts.ApiKey.Events
{
    public record ApiKeyUpdatedEvent(
        string DisplayName,
        string Description,
        string[] Tags) : AbstractGrainUpdatedEvent(
            DisplayName,
            Description,
            Tags);
}
