using Orleans;
using System;
using Xioru.Grain.Contracts.AbstractGrain;

namespace Xioru.Orleans.Tests.Contracts;

[GenerateSerializer]
public record FooUpdatedEvent(
    string DisplayName,
    string Description,
    string[] Tags,
    DateTime UpdatedUtc,
    //
    string FooData,
    string FooMeta) : AbstractGrainUpdatedEvent(
        DisplayName,
        Description,
        Tags,
        UpdatedUtc);
