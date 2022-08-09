using Xioru.Grain.Contracts.AbstractGrain;

namespace Xioru.Orleans.Tests.Contracts
{
    public record UpdateFooCommandModel(
        string DisplayName,
        string Description,
        string[] Tags,
        //
        string FooData) : UpdateAbstractGrainCommandModel(
            DisplayName,
            Description,
            Tags);
}