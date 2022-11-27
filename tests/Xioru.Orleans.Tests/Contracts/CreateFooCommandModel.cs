using Orleans;
using System;
using Xioru.Grain.Contracts.AbstractGrain;

namespace Xioru.Orleans.Tests.Contracts;

[GenerateSerializer]
public record CreateFooCommandModel(
    Guid ProjectId,
    string Name,
    string DisplayName,
    string Description,
    string[] Tags,
    //
    string FooData,
    string FooMeta) : CreateAbstractGrainCommandModel(
        ProjectId,
        Name,
        DisplayName,
        Description,
        Tags);