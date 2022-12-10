using Xioru.Grain.Contracts.Messages;

namespace Xioru.Grain.Contracts.AbstractGrain;

[GenerateSerializer]
public abstract record AbstractGrainUpdatedEvent(
    string DisplayName,
    string Description,
    string[] Tags,
    DateTime UpdatedUtc) : GrainUpdatedEvent(UpdatedUtc);
