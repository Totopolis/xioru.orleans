using Orleans;
using Xioru.Grain.AbstractGrain;
using Xioru.Orleans.Tests.Contracts;

namespace Xioru.Orleans.Tests.Domain;

public class CreateFooValidator :
    CreateAbstractGrainValidator<CreateFooCommandModel>
{
    public CreateFooValidator(IGrainFactory factory)
        : base(factory)
    {
    }
}
