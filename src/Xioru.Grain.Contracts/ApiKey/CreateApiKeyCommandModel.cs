using Xioru.Grain.Contracts.AbstractGrain;

namespace Xioru.Grain.Contracts.ApiKey
{
    public record CreateApiKeyCommandModel(
        Guid ProjectId,
        string Name,
        string DisplayName,
        string Description,
        string[] Tags) : CreateAbstractGrainCommandModel(
            ProjectId,
            Name,
            DisplayName,
            Description,
            Tags);
}