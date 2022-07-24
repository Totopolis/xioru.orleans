namespace Xioru.Grain.Contracts
{
    public class GrainMessage
    {
        public Guid ProjectId { get; set; } = Guid.Empty;

        public string BaseGrainType { get; set; } = default!;

        public string GrainType { get; set; } = default!;

        public Guid GrainId { get; set; } = Guid.Empty;

        public string GrainName { get; set; } = default!;

        public MessageKind Kind { get; set; } = MessageKind.Other;

        public DateTime CreatedUtc { get; set; }

        public string BaseEventType { get; set; } = default!;

        public string EventType { get; set; } = default!;

        public string EventBody { get; set; } = default!;

        public enum MessageKind
        {
            Create,
            Update,
            Delete,
            Other
        }
    }
}
