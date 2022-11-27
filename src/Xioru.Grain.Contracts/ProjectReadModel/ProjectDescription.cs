namespace Xioru.Grain.Contracts.ProjectReadModel;

[GenerateSerializer]
public record ProjectDescription(
    Guid Id,
    string Name);
