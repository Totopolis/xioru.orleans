using Xioru.Grain.Contracts.AbstractGrain;

namespace Xioru.Grain.Contracts.ApiKey;

[GenerateSerializer]
public record ApiKeyProjection(
    Guid Id,
    string Name,
    Guid ProjectId,
    string DisplayName,
    string Description,
    string[] Tags,
    DateTime CreatedUtc,
    DateTime UpdatedUtc,
    //
    Guid ApiKey) : AbstractGrainProjection(
        Id,
        Name,
        ProjectId,
        DisplayName,
        Description,
        Tags,
        CreatedUtc,
        UpdatedUtc);
