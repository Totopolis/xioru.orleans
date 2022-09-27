using Xioru.Grain.Contracts.ProjectReadModel;

namespace Xioru.Grain.Contracts.Account;

public record AccountProjection(
    string AccountId,
    ProjectDescription[] AccessibleProjects);
