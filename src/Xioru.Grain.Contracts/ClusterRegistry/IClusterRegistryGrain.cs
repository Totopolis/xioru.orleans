namespace Xioru.Grain.Contracts.ClusterRegistry;

public interface IClusterRegistryGrain : IGrainWithGuidKey
{
    Task<string?> GetProjectNameByIdOrDefaultAsync(Guid projectId);

    Task<Guid?> GetProjectIdByNameOrDefaultAsync(string projectName);

    Task<ProjectDescription[]> GetProjectsByFilter(string projectNameFilter);
}
