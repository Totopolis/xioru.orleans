namespace Xioru.Grain.Contracts.AbstractGrain
{
    public abstract record AbstractGrainProjection(
        string Name,
        Guid ProjectId,
        string DisplayName,
        string Description,
        string[] Tags);
}
