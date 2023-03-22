namespace Xioru.Grain.Contracts.ClusterRegistry;

[GenerateSerializer]
public record ProjectDescription(
    Guid Id,
    string Name);
