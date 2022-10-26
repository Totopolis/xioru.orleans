namespace Xioru.Grain.Contracts.Project;

public record CreateProjectCommand(
    string Name,
    string DisplayName,
    string Description);
