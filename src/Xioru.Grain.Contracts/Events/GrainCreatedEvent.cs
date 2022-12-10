namespace Xioru.Grain.Contracts.Messages;

[GenerateSerializer]
public abstract record class GrainCreatedEvent(DateTime CreatedUtc) : GrainEvent;
