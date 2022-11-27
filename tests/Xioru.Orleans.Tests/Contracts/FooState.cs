using Orleans;
using Xioru.Grain.AbstractGrain;

namespace Xioru.Orleans.Tests.Contracts;

[GenerateSerializer]
public class FooState : AbstractGrainState
{
    public string FooData { get; set; } = default!;

    public string FooMeta { get; set; } = default!;
}
