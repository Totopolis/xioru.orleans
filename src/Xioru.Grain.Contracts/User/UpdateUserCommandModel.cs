using Xioru.Grain.Contracts.AbstractGrain;

namespace Xioru.Grain.Contracts.User;

[GenerateSerializer]
public record UpdateUserCommandModel(
    string DisplayName,
    string Description,
    string[] Tags) : UpdateAbstractGrainCommandModel(
        DisplayName,
        Description,
        Tags);