using MongoDB.Bson.Serialization.Attributes;

namespace Xioru.Grain.ApiKeyReadModel;

internal class ApiKeyDocument
{
    [BsonId]
    public Guid ApiKey { get; set; }

    public DateTime Created { get; set; }

    public Guid ProjectId { get; set; }

    public Guid GrainId { get; set; }
}
