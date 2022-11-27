namespace Xioru.Grain.Contracts.Messages;

[GenerateSerializer]
public abstract record class GrainEvent : IGrainEvent
{
    [Id(0)]
    public GrainEventMetadata? Metadata { get; set; }
}
