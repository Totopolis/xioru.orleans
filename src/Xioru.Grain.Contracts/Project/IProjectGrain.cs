using Orleans;

namespace Xioru.Grain.Contracts.Project;

public interface IProjectGrain : IGrainWithGuidKey
{
    Task Create(CreateProjectCommand createCommand);

    Task Delete();

    Task<ProjectProjection> GetProjection();
}
