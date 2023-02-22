namespace Xioru.Grain.Contracts.ClusterRegistry;

public interface IClusterRegistryGrain : IGrainWithGuidKey
{
    Task<string?> GetProjectNameByIdOrDefaultAsync(Guid projectId);

    Task<Guid?> GetProjectIdByNameOrDefaultAsync(string projectName);
}

public interface IClusterRegistryWriterGrain : IGrainWithGuidKey
{
    Task<bool> OnProjectCreatedAsync(Guid projectId, string name);

    Task<bool> OnProjectDeletedAsync(Guid projectId, string name);
}
