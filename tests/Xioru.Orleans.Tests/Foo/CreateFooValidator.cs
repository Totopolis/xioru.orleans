using Orleans;
using Xioru.Grain.AbstractGrain;

namespace Xioru.Orleans.Tests.Foo
{
    public class CreateFooValidator :
        CreateAbstractGrainValidator<CreateFooCommandModel>
    {
        public CreateFooValidator(IGrainFactory factory)
            : base(factory)
        {
        }
    }
}
