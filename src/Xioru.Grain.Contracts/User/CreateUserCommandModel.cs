using Xioru.Grain.Contracts.AbstractGrain;

namespace Xioru.Grain.Contracts.User;

[GenerateSerializer]
public record CreateUserCommandModel(
    Guid ProjectId,
    string Name,
    string DisplayName,
    string Description,
    string[] Tags,
    //
    string Login) : CreateAbstractGrainCommandModel(
        ProjectId,
        Name,
        DisplayName,
        Description,
        Tags);