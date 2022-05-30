using Orleans;
using Xioru.Grain.AbstractGrain;
using Xioru.Grain.Contracts.User;

namespace Xioru.Grain.User
{
    public class UpdateUserValidator :
        UpdateAbstractGrainValidator<UpdateUserCommand>
    {
        public UpdateUserValidator(IGrainFactory factory)
            : base(factory)
        {
        }
    }
}
