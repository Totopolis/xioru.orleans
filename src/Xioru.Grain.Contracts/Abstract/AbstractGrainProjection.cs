namespace Xioru.Grain.Contracts.AbstractGrain
{
    public abstract record AbstractGrainProjection(
        Guid Id,
        string Name,
        Guid ProjectId,
        string DisplayName,
        string Description,
        string[] Tags);
}
