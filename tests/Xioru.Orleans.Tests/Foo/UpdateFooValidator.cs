using Orleans;
using Xioru.Grain.AbstractGrain;

namespace Xioru.Orleans.Tests.Foo
{
    public class UpdateFooValidator :
        UpdateAbstractGrainValidator<UpdateFooCommandModel>
    {
        public UpdateFooValidator(IGrainFactory factory)
            : base(factory)
        {
        }
    }
}
