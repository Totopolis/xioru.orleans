using Xioru.Grain.Contracts.ClusterRegistry;

namespace Xioru.Grain.Contracts.Account;

[GenerateSerializer]
public record AccountProjection(
    string AccountId,
    ProjectDescription[] AccessibleProjects);
