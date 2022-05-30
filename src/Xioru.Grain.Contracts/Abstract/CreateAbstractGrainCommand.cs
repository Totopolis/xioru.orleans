namespace Xioru.Grain.Contracts.AbstractGrain
{
    public abstract record CreateAbstractGrainCommand(
        Guid ProjectId,
        string Name,
        string DisplayName,
        string Description,
        string[] Tags);
}
