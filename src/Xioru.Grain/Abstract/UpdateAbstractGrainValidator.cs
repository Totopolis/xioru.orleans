using FluentValidation;
using Orleans;
using Xioru.Grain.Contracts.AbstractGrain;

namespace Xioru.Grain.AbstractGrain
{
    public abstract class UpdateAbstractGrainValidator<T> : AbstractValidator<T>
        where T : UpdateAbstractGrainCommand
    {
        public UpdateAbstractGrainValidator(IGrainFactory factory)
        {
        }
    }
}
