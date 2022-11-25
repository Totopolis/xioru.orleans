using Orleans;
using Xioru.Grain.Contracts.AbstractGrain;

namespace Xioru.Orleans.Tests.Contracts;

[GenerateSerializer]
public record FooCreatedEvent(
    string DisplayName,
    string Description,
    string[] Tags,
    //
    string FooData,
    string FooMeta) : AbstractGrainCreatedEvent(
        DisplayName,
        Description,
        Tags);
