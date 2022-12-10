using Xioru.Grain.Contracts.Messages;

namespace Xioru.Grain.Contracts.Project.Events;

[GenerateSerializer]
public record ProjectUpdatedEvent(DateTime UpdatedUtc) : GrainCreatedEvent(UpdatedUtc);
