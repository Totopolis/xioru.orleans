using MongoDB.Bson.Serialization.Attributes;

namespace Xioru.Grain.GrainReadModel.State;

[BsonDiscriminator(nameof(GrainDetailsDocument))]
public class GrainDetailsDocument
{
    public string GrainName { get; set; } = default!;

    public string GrainType { get; set; } = default!;

    [BsonId]
    public Guid GrainId { get; set; } = default!;
}
