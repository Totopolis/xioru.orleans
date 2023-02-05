using Orleans;
using Xioru.Grain.AbstractGrain;

namespace Xioru.Orleans.Tests.Contracts;

[GenerateSerializer]
public class FooState : AbstractGrainState
{
    [Id(0)]
    public string FooData { get; set; } = default!;

    [Id(1)]
    public string FooMeta { get; set; } = default!;
}
