using Orleans;

namespace Xioru.Grain.Contracts.ProjectReadModel;

public interface IProjectReadModelGrain : IGrainWithGuidKey
{
    Task<ProjectDescription?> GetProjectById(Guid projectId);

    Task<ProjectDescription?> GetProjectByName(string projectName);

    Task<ProjectDescription[]> GetProjectsByFilter(string projectNameFilter);
}
