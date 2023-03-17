using FluentValidation;
using Xioru.Grain.Contracts.AbstractGrain;

namespace Xioru.Grain.AbstractGrain;

public abstract class UpdateAbstractGrainValidator<T> : AbstractValidator<T>
    where T : UpdateAbstractGrainCommandModel
{
    public UpdateAbstractGrainValidator(IGrainFactory factory)
    {
    }
}
