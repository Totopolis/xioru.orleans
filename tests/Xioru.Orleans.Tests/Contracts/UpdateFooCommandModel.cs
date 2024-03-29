﻿using Orleans;
using Xioru.Grain.Contracts.AbstractGrain;

namespace Xioru.Orleans.Tests.Contracts;

[GenerateSerializer]
public record UpdateFooCommandModel(
    string DisplayName,
    string Description,
    string[] Tags,
    //
    string FooData,
    string FooMeta) : UpdateAbstractGrainCommandModel(
        DisplayName,
        Description,
        Tags);