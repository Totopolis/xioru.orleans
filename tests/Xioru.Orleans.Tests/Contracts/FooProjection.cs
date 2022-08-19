using System;
using Xioru.Grain.Contracts.AbstractGrain;

namespace Xioru.Orleans.Tests.Contracts
{
    public record FooProjection(
        Guid Id,
        string Name,
        Guid ProjectId,
        string DisplayName,
        string Description,
        string[] Tags,
        //
        string FooData,
        string FooMeta) : AbstractGrainProjection(
            Id,
            Name,
            ProjectId,
            DisplayName,
            Description,
            Tags);
}