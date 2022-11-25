namespace Xioru.Grain.Contracts.Project;

[GenerateSerializer]
public record CreateProjectCommand(
    string Name,
    string DisplayName,
    string Description);
