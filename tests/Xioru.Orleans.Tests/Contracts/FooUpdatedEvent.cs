using Orleans;
using Xioru.Grain.Contracts.AbstractGrain;

namespace Xioru.Orleans.Tests.Contracts;

[GenerateSerializer]
public record FooUpdatedEvent(
    string DisplayName,
    string Description,
    string[] Tags,
    //
    string FooData,
    string FooMeta) : AbstractGrainUpdatedEvent(
        DisplayName,
        Description,
        Tags);
