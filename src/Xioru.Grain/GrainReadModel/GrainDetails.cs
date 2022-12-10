using MongoDB.Bson.Serialization.Attributes;

namespace Xioru.Grain.GrainReadModel;

[BsonDiscriminator(nameof(GrainDetailsDocument))]
public class GrainDetailsDocument
{
    public string GrainName { get; set; } = default!;

    public string GrainType { get; set; } = default!;

    [BsonId]
    public Guid GrainId { get; set; } = default!;

    public DateTime CreatedUtc { get; set; } = DateTime.MinValue;

    public DateTime UpdatedUtc { get; set; } = DateTime.MinValue;
}
