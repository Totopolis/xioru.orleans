using FluentValidation;
using Orleans;
using Xioru.Grain.Contracts.AbstractGrain;
using Xioru.Grain.Contracts.GrainReadModel;

namespace Xioru.Grain.AbstractGrain;

public abstract class CreateAbstractGrainValidator<T> : AbstractValidator<T>
    where T : CreateAbstractGrainCommandModel
{
    public CreateAbstractGrainValidator(IGrainFactory factory)
    {
        // string.IsNullOrWhiteSpace(createCommand.Name)
        RuleFor(c => c.Name)
            .NotNull()
            .NotEmpty()
            .WithName("Grain name")
            .WithMessage("Empty string");

        // TODO: check Name not exists in projectModel

        RuleFor(c => c.ProjectId)
            .MustAsync(async (projectId, cancel) =>
            {
                var projectReadModel = factory
                    .GetGrain<IGrainReadModelGrain>(projectId);
                var projectDescription = await projectReadModel
                    .GetGrainById(projectId);

                return projectDescription != null;
            })
            .WithName("Grain project")
            .WithMessage("ProjectId not found");

    }
}
