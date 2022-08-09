using System;
using Xioru.Grain.Contracts.AbstractGrain;

namespace Xioru.Orleans.Tests.Contracts
{
    public record CreateFooCommandModel(
        Guid ProjectId,
        string Name,
        string DisplayName,
        string Description,
        string[] Tags,
        //
        string FooData) : CreateAbstractGrainCommandModel(
            ProjectId,
            Name,
            DisplayName,
            Description,
            Tags);
}