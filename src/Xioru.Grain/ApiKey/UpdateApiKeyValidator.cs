using Orleans;
using Xioru.Grain.AbstractGrain;
using Xioru.Grain.Contracts.ApiKey;

namespace Xioru.Grain.ApiKey
{
    public class UpdateApiKeyValidator :
        UpdateAbstractGrainValidator<UpdateApiKeyCommand>
    {
        public UpdateApiKeyValidator(IGrainFactory factory)
            : base(factory)
        {
        }
    }
}
