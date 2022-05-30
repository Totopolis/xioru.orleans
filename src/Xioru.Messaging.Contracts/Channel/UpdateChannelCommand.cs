using Xioru.Grain.Contracts.AbstractGrain;

namespace Xioru.Messaging.Contracts.Channel
{
    public record UpdateChannelCommand(
        string DisplayName,
        string Description,
        string[] Tags) : UpdateAbstractGrainCommand(
            DisplayName,
            Description,
            Tags);
}