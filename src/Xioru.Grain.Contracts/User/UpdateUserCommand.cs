using Xioru.Grain.Contracts.AbstractGrain;

namespace Xioru.Grain.Contracts.User
{
    public record UpdateUserCommand(
        string DisplayName,
        string Description,
        string[] Tags) : UpdateAbstractGrainCommand(
            DisplayName,
            Description,
            Tags);
}