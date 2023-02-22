namespace Xioru.Grain.Contracts.ProjectRegistry;

public interface IProjectRegistryGrain : IGrainWithGuidKey
{
    /// <summary>
    /// Get grain details by name
    /// </summary>
    /// <returns>
    /// Return description (name, type, id) or null if not found
    /// </returns>
    Task<GrainRegistryDetails?> GetGrainDetailsByName(string name);

    /// <summary>
    /// Get grain details by name
    /// </summary>
    /// <returns>
    /// Return description (name, type, id) or null if not found
    /// </returns>
    Task<GrainRegistryDetails?> GetGrainDetailsById(Guid id);

    Task<IReadOnlyCollection<GrainRegistryDetails>> GetGrains();
}
