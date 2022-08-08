using FluentValidation;
using Orleans;
using Xioru.Grain.AbstractGrain;
using Xioru.Messaging.Contracts.Channel;

namespace Xioru.Messaging.Channel
{
    public class CreateChannelValidator : CreateAbstractGrainValidator<CreateChannelCommandModel>
    {
        public CreateChannelValidator(IGrainFactory factory)
            : base(factory)
        {
            RuleFor(c => c.ChatId)
                .NotNull()
                .NotEmpty();
        }
    }
}
