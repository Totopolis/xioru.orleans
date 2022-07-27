using MongoDB.Bson.Serialization.Attributes;
using Ofg.Core.Contracts.Handler;
using Ofg.Core.Contracts.Variable;

namespace Xioru.Grain.GrainReadModel
{
    [BsonDiscriminator(nameof(TechnoGrainInfo))]
    public class TechnoGrainInfo : GrainInfo
    {
        public string DisplayName { get; set; }

        public string Description { get; set; }

        public string[] Tags { get; set; }

        public Guid? ParentTemplate { get; set; }

        public bool IsTemplate { get; set; }

        public VariableDefinition[] EmbeddedVariables { get; set; }

        public VariableDefinition[] TemplatedVariables { get; set; }

        public VariableDefinition[] Variables { get; set; }

        public HandlerDefinition[] TemplatedHandlers { get; set; }

        public HandlerDefinition[] Handlers { get; set; }
    }
}
