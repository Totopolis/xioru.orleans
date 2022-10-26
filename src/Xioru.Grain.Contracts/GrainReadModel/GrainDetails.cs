namespace Xioru.Grain.Contracts.GrainReadModel;

public record GrainDetails(
    string GrainName,
    string GrainType,
    Guid GrainId);
