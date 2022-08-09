﻿using Xioru.Grain.Contracts.AbstractGrain;

namespace Xioru.Orleans.Tests.Contracts
{
    public record FooUpdatedEvent(
        string DisplayName,
        string Description,
        string[] Tags,
        //
        string FooData) : AbstractGrainUpdatedEvent(
            DisplayName,
            Description,
            Tags);
}