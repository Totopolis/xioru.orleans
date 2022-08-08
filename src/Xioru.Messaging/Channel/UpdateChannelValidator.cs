using Orleans;
using Xioru.Grain.AbstractGrain;
using Xioru.Messaging.Contracts.Channel;

namespace Xioru.Messaging.Channel
{
    public class UpdateChannelValidator : UpdateAbstractGrainValidator<UpdateChannelCommandModel>
    {
        public UpdateChannelValidator(IGrainFactory factory)
            : base(factory)
        {
        }
    }
}
