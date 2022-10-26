using Orleans;
using Xioru.Grain.AbstractGrain;
using Xioru.Grain.Contracts.User;

namespace Xioru.Grain.User;

public class CreateUserValidator :
    CreateAbstractGrainValidator<CreateUserCommandModel>
{
    public CreateUserValidator(IGrainFactory factory)
        : base(factory)
    {
    }
}
