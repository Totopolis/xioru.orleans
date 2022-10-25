using Xioru.Grain.Contracts.Messages;

namespace Xioru.Grain.Contracts.AbstractGrain;

public abstract record AbstractGrainUpdatedEvent(
    string DisplayName,
    string Description,
    string[] Tags) : GrainUpdatedEvent;
