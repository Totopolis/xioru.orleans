namespace Xioru.Grain.Contracts.GrainReadModel;

[GenerateSerializer]
public record GrainDetails(
    string GrainName,
    string GrainType,
    Guid GrainId);
