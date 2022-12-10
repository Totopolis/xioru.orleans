using Xioru.Grain.Contracts.AbstractGrain;

namespace Xioru.Messaging.Contracts.Channel.Events;

[GenerateSerializer]
public record ChannelUpdatedEvent(
    string DisplayName,
    string Description,
    string[] Tags,
    DateTime UpdatedUtc) : AbstractGrainUpdatedEvent(
        DisplayName,
        Description,
        Tags,
        UpdatedUtc);
