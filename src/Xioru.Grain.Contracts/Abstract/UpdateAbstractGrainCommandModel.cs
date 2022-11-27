namespace Xioru.Grain.Contracts.AbstractGrain;

[GenerateSerializer]
public abstract record UpdateAbstractGrainCommandModel(
    string DisplayName,
    string Description,
    string[] Tags);
