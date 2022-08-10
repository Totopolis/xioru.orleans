using Orleans;
using Xioru.Grain.AbstractGrain;
using Xioru.Orleans.Tests.Contracts;

namespace Xioru.Orleans.Tests.Domain
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
