using Orleans;
using Xioru.Grain.AbstractGrain;
using Xioru.Grain.Contracts.ApiKey;

namespace Xioru.Grain.ApiKey
{
    public class CreateApiKeyValidator :
        CreateAbstractGrainValidator<CreateApiKeyCommand>
    {
        public CreateApiKeyValidator(IGrainFactory factory)
            : base(factory)
        {
        }
    }
}
