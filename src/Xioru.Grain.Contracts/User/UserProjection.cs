using Xioru.Grain.Contracts.AbstractGrain;

namespace Xioru.Grain.Contracts.User
{
    public record UserProjection(
        string Name,
        Guid ProjectId,
        string DisplayName,
        string Description,
        string[] Tags,
        //
        string Login) : AbstractGrainProjection(
            Name,
            ProjectId,
            DisplayName,
            Description,
            Tags);
}