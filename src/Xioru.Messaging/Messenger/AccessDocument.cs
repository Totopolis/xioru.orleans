using MongoDB.Bson.Serialization.Attributes;
using Xioru.Messaging.Contracts.Messenger;

namespace Xioru.Messaging.Messenger
{
    internal class AccessDocument
    {
        [BsonId] //TODO: consider making objectId
        public Guid Id { get; set; }

        public string ChatId { get; set; } = default!;

        public Guid ChannelId { get; set; } = Guid.Empty;

        public string ProjectName { get; set; } = default!;

        public Guid ProjectId { get; set; } = Guid.Empty;

        public bool IsCurrent { get; set; } = false;
    }
}
