namespace Xioru.Grain.Contracts.Project;

[GenerateSerializer]
public record ProjectProjection(
    string Name,
    string DisplayName,
    string Description);
