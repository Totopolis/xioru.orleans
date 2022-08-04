namespace Xioru.Grain.Contracts.AbstractGrain
{
    public abstract record CreateAbstractGrainCommandModel(
        Guid ProjectId,
        string Name,
        string DisplayName,
        string Description,
        string[] Tags);
}
