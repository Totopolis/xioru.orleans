using Xioru.Grain.Contracts.AbstractGrain;

namespace Xioru.Grain.Contracts.ApiKey
{
    public record ApiKeyProjection(
        string Name,
        Guid ProjectId,
        string DisplayName,
        string Description,
        string[] Tags,
        //
        DateTime Created,
        Guid ApiKey) : AbstractGrainProjection(
            Name,
            ProjectId,
            DisplayName,
            Description,
            Tags);
}