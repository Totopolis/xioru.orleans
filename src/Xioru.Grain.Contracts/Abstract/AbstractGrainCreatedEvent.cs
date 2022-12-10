using Xioru.Grain.Contracts.Messages;

namespace Xioru.Grain.Contracts.AbstractGrain;

[GenerateSerializer]
public abstract record AbstractGrainCreatedEvent(
    string DisplayName,
    string Description,
    string[] Tags,
    DateTime CreatedUtc) : GrainCreatedEvent(CreatedUtc);
