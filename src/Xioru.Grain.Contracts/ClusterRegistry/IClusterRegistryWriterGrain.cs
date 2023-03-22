namespace Xioru.Grain.Contracts.ClusterRegistry;

public interface IClusterRegistryWriterGrain : IGrainWithGuidKey
{
    Task<bool> OnProjectCreatedAsync(Guid projectId, string name);

    Task<bool> OnProjectDeletedAsync(Guid projectId, string name);
}
