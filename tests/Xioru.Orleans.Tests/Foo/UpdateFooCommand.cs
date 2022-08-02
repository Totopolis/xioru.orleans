using Xioru.Grain.Contracts.AbstractGrain;

namespace Xioru.Orleans.Tests.Foo
{
    public record UpdateFooCommand(
        string DisplayName,
        string Description,
        string[] Tags,
        //
        string FooData) : UpdateAbstractGrainCommand(
            DisplayName,
            Description,
            Tags);
}