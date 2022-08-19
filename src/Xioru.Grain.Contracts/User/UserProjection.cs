using Xioru.Grain.Contracts.AbstractGrain;

namespace Xioru.Grain.Contracts.User
{
    public record UserProjection(
        Guid Id,
        string Name,
        Guid ProjectId,
        string DisplayName,
        string Description,
        string[] Tags,
        //
        string Login) : AbstractGrainProjection(
            Id,
            Name,
            ProjectId,
            DisplayName,
            Description,
            Tags);
}