using Xioru.Grain.Contracts.AbstractGrain;

namespace Xioru.Grain.Contracts.User
{
    public record CreateUserCommand(
        Guid ProjectId,
        string Name,
        string DisplayName,
        string Description,
        string[] Tags,
        //
        string Login) : CreateAbstractGrainCommand(
            ProjectId,
            Name,
            DisplayName,
            Description,
            Tags);
}