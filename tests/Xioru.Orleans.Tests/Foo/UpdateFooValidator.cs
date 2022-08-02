using Orleans;
using Xioru.Grain.AbstractGrain;

namespace Xioru.Orleans.Tests.Foo
{
    public class UpdateFooValidator :
        UpdateAbstractGrainValidator<UpdateFooCommand>
    {
        public UpdateFooValidator(IGrainFactory factory)
            : base(factory)
        {
        }
    }
}
