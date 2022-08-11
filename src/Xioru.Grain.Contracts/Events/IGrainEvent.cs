namespace Xioru.Grain.Contracts.Messages
{
    public interface IGrainEvent
    {
        GrainEventMetadata? Metadata { get; set; }
    }
}