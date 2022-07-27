using Xioru.Grain.Contracts.Messages;

namespace Xioru.Grain.Contracts.AbstractGrain
{
    public abstract record AbstractGrainCreatedEvent(
        string DisplayName,
        string Description,
        string[] Tags) : GrainCreatedEvent;
}
