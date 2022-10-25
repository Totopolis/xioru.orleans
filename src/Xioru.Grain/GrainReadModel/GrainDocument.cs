using MongoDB.Bson.Serialization.Attributes;

namespace Xioru.Grain.GrainReadModel;

internal class GrainDocument
{
    public string GrainName { get; set; } = default!;

    public string GrainType { get; set; } = default!;

    [BsonId]
    public Guid GrainId { get; set; } = default!;
}
