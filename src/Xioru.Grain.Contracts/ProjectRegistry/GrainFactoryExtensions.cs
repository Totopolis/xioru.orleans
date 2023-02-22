namespace Xioru.Grain.Contracts.ProjectRegistry;

public static class GrainFactoryExtensions
{
    public async static Task<bool> CheckGrainExistsInProjectAsync(this IGrainFactory grainFactory, Guid projectId, string grainName)
    {
        var projectRegistry = grainFactory.GetGrain<IProjectRegistryGrain>(projectId);
        var grainDetails = await projectRegistry.GetGrainDetailsByName(grainName);
        return grainDetails != default;
    }

    public async static Task<bool> CheckGrainExistsInProjectAsync(this IGrainFactory grainFactory, Guid projectId, Guid grainId)
    {
        var projectRegistry = grainFactory.GetGrain<IProjectRegistryGrain>(projectId);
        var grainDetails = await projectRegistry.GetGrainDetailsById(grainId);
        return grainDetails != default;
    }

    public async static Task<bool> CheckGrainExistsInProjectAsync<T>(this IGrainFactory grainFactory, Guid projectId, Guid grainId)
    {
        var typeName = typeof(T).FullName;
        var projectRegistry = grainFactory.GetGrain<IProjectRegistryGrain>(projectId);
        var grainDetails = await projectRegistry.GetGrainDetailsById(grainId);
        return grainDetails != null && grainDetails.GrainType == typeName;
    }

    public async static Task<T> GetGrainFromProjectAsync<T>(this IGrainFactory grainFactory, Guid projectId, Guid grainId)
        where T: IGrainWithGuidKey
    {
        var typeName = typeof(T).FullName;
        var projectRegistry = grainFactory.GetGrain<IProjectRegistryGrain>(projectId);
        var grainDetails = await projectRegistry.GetGrainDetailsById(grainId);
        if (grainDetails == null || grainDetails.GrainType == typeName)
        {
            throw new ArgumentException($"Grain {grainId} of type {typeName} has not been found in project {projectId}");
        }

        return grainFactory.GetGrain<T>(grainId);
    }

    public async static Task<T> GetGrainFromProjectAsync<T>(this IGrainFactory grainFactory, Guid projectId, string grainName)
    where T : IGrainWithGuidKey
    {
        var typeName = typeof(T).FullName;
        var projectRegistry = grainFactory.GetGrain<IProjectRegistryGrain>(projectId);
        var grainDetails = await projectRegistry.GetGrainDetailsByName(grainName);
        if (grainDetails == null || grainDetails.GrainType == typeName)
        {
            throw new ArgumentException($"Grain {grainName} of type {typeName} has not been found in project {projectId}");
        }

        return grainFactory.GetGrain<T>(grainDetails.GrainId);
    }
}
