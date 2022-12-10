using Orleans;
using System;
using Xioru.Grain.Contracts.AbstractGrain;

namespace Xioru.Orleans.Tests.Contracts;

[GenerateSerializer]
public record FooProjection(
    Guid Id,
    string Name,
    Guid ProjectId,
    string DisplayName,
    string Description,
    string[] Tags,
    DateTime CreatedUtc,
    DateTime UpdatedUtc,
    //
    string FooData,
    string FooMeta) : AbstractGrainProjection(
        Id,
        Name,
        ProjectId,
        DisplayName,
        Description,
        Tags,
        CreatedUtc,
        UpdatedUtc);
