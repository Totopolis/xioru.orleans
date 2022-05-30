using Xioru.Grain.Contracts.AbstractGrain;

namespace Xioru.Grain.Contracts.ApiKey
{
    public record CreateApiKeyCommand(
        Guid ProjectId,
        string Name,
        string DisplayName,
        string Description,
        string[] Tags) : CreateAbstractGrainCommand(
            ProjectId,
            Name,
            DisplayName,
            Description,
            Tags);
}