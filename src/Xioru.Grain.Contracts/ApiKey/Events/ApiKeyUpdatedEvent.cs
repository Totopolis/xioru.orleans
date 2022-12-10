using Xioru.Grain.Contracts.AbstractGrain;

namespace Xioru.Grain.Contracts.ApiKey.Events;

public record ApiKeyUpdatedEvent(
    string DisplayName,
    string Description,
    string[] Tags,
    DateTime UpdatedUtc) : AbstractGrainUpdatedEvent(
        DisplayName,
        Description,
        Tags,
        UpdatedUtc);
