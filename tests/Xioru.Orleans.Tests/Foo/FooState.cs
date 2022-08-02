using Xioru.Grain.AbstractGrain;

namespace Xioru.Orleans.Tests.Foo
{
    public class FooState : AbstractGrainState
    {
        public string FooData { get; set; } = default!;
    }
}
