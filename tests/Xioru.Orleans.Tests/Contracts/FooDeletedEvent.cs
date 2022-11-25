using Orleans;
using Xioru.Grain.Contracts.Messages;

namespace Xioru.Orleans.Tests.Contracts;

[GenerateSerializer]
public record FooDeletedEvent : GrainDeletedEvent
{
}
