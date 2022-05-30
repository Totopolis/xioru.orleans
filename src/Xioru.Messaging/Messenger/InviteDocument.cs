using MongoDB.Bson.Serialization.Attributes;

namespace Xioru.Messaging.Messenger
{
    internal class InviteDocument
    {
        [BsonId]
        public Guid Id { get; set; }

        public DateTime Created { get; set; }

        public string Code { get; set; } = default!;

        public Guid ProjectId { get; set; }
    }
}
