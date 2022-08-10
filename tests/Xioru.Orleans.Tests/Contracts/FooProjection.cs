using System;
using Xioru.Grain.Contracts.AbstractGrain;

namespace Xioru.Orleans.Tests.Contracts
{
    public record FooProjection(
        string Name,
        Guid ProjectId,
        string DisplayName,
        string Description,
        string[] Tags,
        //
        string FooData,
        string FooMeta) : AbstractGrainProjection(
            Name,
            ProjectId,
            DisplayName,
            Description,
            Tags);
}