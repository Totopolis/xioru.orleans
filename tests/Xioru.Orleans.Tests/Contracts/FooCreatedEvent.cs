﻿using Xioru.Grain.Contracts.AbstractGrain;

namespace Xioru.Orleans.Tests.Contracts
{
    public record FooCreatedEvent(
        string DisplayName,
        string Description,
        string[] Tags,
        //
        string FooData) : AbstractGrainCreatedEvent(
            DisplayName,
            Description,
            Tags);
}