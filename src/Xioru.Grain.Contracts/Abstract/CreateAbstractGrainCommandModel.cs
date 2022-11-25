namespace Xioru.Grain.Contracts.AbstractGrain;

[GenerateSerializer]
public abstract record CreateAbstractGrainCommandModel(
    Guid ProjectId,
    string Name,
    string DisplayName,
    string Description,
    string[] Tags);
