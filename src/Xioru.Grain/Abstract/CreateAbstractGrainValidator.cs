using FluentValidation;
using Xioru.Grain.Contracts.AbstractGrain;
using Xioru.Grain.Contracts.GrainReadModel;
using Xioru.Grain.Contracts.ProjectRegistry;

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
                var projectRegistry = factory
                    .GetGrain<IProjectRegistryGrain>(projectId);
                var projectDescription = await projectRegistry!
                    .GetGrainDetailsById(projectId);

                return projectDescription != null;
            })
            .WithName("Grain project")
            .WithMessage("ProjectId not found");
    }
}
