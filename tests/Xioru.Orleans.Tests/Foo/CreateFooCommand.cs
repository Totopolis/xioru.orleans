using System;
using Xioru.Grain.Contracts.AbstractGrain;

namespace Xioru.Orleans.Tests.Foo
{
    public record CreateFooCommand(
        Guid ProjectId,
        string Name,
        string DisplayName,
        string Description,
        string[] Tags,
        //
        string FooData) : CreateAbstractGrainCommand(
            ProjectId,
            Name,
            DisplayName,
            Description,
            Tags);
}