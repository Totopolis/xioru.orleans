using Orleans;
using Xioru.Grain.AbstractGrain;
using Xioru.Grain.Contracts.ApiKey;

namespace Xioru.Grain.ApiKey
{
    public class CreateApiKeyValidator :
        CreateAbstractGrainValidator<CreateApiKeyCommandModel>
    {
        public CreateApiKeyValidator(IGrainFactory factory)
            : base(factory)
        {
        }
    }
}
