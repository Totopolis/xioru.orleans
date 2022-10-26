using Xioru.Grain.Contracts.AbstractGrain;

namespace Xioru.Messaging.Contracts.Channel;

public record UpdateChannelCommandModel(
    string DisplayName,
    string Description,
    string[] Tags) : UpdateAbstractGrainCommandModel(
        DisplayName,
        Description,
        Tags);