using Xioru.Grain.Contracts.AbstractGrain;

namespace Xioru.Grain.Contracts.User;

[GenerateSerializer]
public record UserProjection(
    Guid Id,
    string Name,
    Guid ProjectId,
    string DisplayName,
    string Description,
    string[] Tags,
    DateTime CreatedUtc,
    DateTime UpdatedUtc,
    //
    string Login) : AbstractGrainProjection(
        Id,
        Name,
        ProjectId,
        DisplayName,
        Description,
        Tags,
        CreatedUtc,
        UpdatedUtc);