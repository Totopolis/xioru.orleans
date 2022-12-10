namespace Xioru.Grain.Contracts.AbstractGrain;

[GenerateSerializer]
public abstract record AbstractGrainProjection(
    Guid Id,
    string Name,
    Guid ProjectId,
    string DisplayName,
    string Description,
    string[] Tags,
    DateTime CreatedUtc,
    DateTime UpdatedUtc);
