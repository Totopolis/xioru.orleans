using Xioru.Grain.Contracts.AbstractGrain;

namespace Xioru.Grain.Contracts.ApiKey.Events
{
    public record ApiKeyCreatedEvent(
        string DisplayName,
        string Description,
        string[] Tags,
        //
        DateTime Created,
        Guid ApiKey) : AbstractGrainCreatedEvent(
            DisplayName,
            Description,
            Tags);
}
