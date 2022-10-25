using Xioru.Grain.Contracts.AbstractGrain;

namespace Xioru.Grain.Contracts.ApiKey;

public record UpdateApiKeyCommandModel(
    string DisplayName,
    string Description,
    string[] Tags) : UpdateAbstractGrainCommandModel(
        DisplayName,
        Description,
        Tags);