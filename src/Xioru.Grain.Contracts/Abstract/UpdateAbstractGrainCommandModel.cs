namespace Xioru.Grain.Contracts.AbstractGrain;

public abstract record UpdateAbstractGrainCommandModel(
    string DisplayName,
    string Description,
    string[] Tags);
