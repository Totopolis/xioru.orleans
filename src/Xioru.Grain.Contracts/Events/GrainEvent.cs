namespace Xioru.Grain.Contracts.Messages
{
    public abstract record class GrainEvent
    {
        public GrainEventMetadata? Metadata { get; set; }
    }
}
