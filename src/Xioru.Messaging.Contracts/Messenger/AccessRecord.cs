namespace Xioru.Messaging.Contracts.Messenger
{
    public class AccessRecord
    {
        public string ChatId { get; set; } = default!;

        public Guid ChannelId { get; set; } = Guid.Empty;

        public string ProjectName { get; set; } = default!;

        public Guid ProjectId { get; set; } = Guid.Empty;

        public bool IsCurrent { get; set; } = false;
    }
}
