namespace Xioru.Grain.Contracts.Messages;

public abstract record class GrainEvent : IGrainEvent
{
    public GrainEventMetadata? Metadata { get; set; }
}
