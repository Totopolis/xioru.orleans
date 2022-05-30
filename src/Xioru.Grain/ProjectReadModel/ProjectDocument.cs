using MongoDB.Bson.Serialization.Attributes;

namespace Xioru.Grain.ProjectReadModel
{
    internal class ProjectDocument
    {
        public string ProjectName { get; set; } = default!;

        [BsonId]
        public Guid ProjectId { get; set; } = Guid.Empty;
    }
}
