using Orleans;
using System;
using Xioru.Grain.Contracts.AbstractGrain;

namespace Xioru.Orleans.Tests.Contracts;

[GenerateSerializer]
public record FooCreatedEvent(
    string DisplayName,
    string Description,
    string[] Tags,
    DateTime CreatedUtc,
    //
    string FooData,
    string FooMeta) : AbstractGrainCreatedEvent(
        DisplayName,
        Description,
        Tags,
        CreatedUtc);
