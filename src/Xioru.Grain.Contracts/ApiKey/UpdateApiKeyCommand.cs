using Xioru.Grain.Contracts.AbstractGrain;

namespace Xioru.Grain.Contracts.ApiKey
{
    public record UpdateApiKeyCommand(
        string DisplayName,
        string Description,
        string[] Tags) : UpdateAbstractGrainCommand(
            DisplayName,
            Description,
            Tags);
}