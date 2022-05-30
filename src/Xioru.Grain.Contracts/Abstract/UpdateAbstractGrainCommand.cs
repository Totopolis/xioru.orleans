namespace Xioru.Grain.Contracts.AbstractGrain
{
    public abstract record UpdateAbstractGrainCommand(
        string DisplayName,
        string Description,
        string[] Tags);
}
