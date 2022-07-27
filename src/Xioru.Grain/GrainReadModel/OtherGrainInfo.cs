using MongoDB.Bson.Serialization.Attributes;

namespace Xioru.Grain.GrainReadModel
{
    [BsonDiscriminator(nameof(OtherGrainInfo))]
    public class OtherGrainInfo : GrainInfo
    {
    }
}
