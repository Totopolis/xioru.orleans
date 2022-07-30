using Xioru.Grain.Contracts.AbstractGrain;

namespace Xioru.Orleans.Tests.Foo
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
