namespace Xioru.Grain.Contracts.ProjectRegistry;

[GenerateSerializer]
public record GrainRegistryDetails(
    string GrainName,
    string GrainType,
    Guid GrainId);
