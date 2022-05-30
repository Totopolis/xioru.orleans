using MongoDB.Bson.Serialization.Attributes;

namespace Xioru.Messaging.Messenger
{
    internal class AccessDocument
    {
        [BsonId]
        public Guid Id { get; set; }

        public Guid MessengerId { get; set; } = Guid.Empty;

        public string ChatId { get; set; } = default!;

        public Guid ChannelId { get; set; } = Guid.Empty;

        public string ProjectName { get; set; } = default!;

        public Guid ProjectId { get; set; } = Guid.Empty;

        public bool IsCurrent { get; set; } = false;
    }
}
