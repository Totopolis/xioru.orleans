using Xioru.Grain.Contracts.ProjectReadModel;

namespace Xioru.Grain.Contracts.Account;

[GenerateSerializer]
public record AccountProjection(
    string AccountId,
    ProjectDescription[] AccessibleProjects);
