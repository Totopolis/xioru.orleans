using Orleans;

namespace Xioru.Grain.Contracts.GrainReadModel
{
    public interface IGrainReadModelGrain : IGrainWithGuidKey
    {
        /// <summary>
        /// Count of grains in projectId
        /// </summary>
        Task<long> GrainsCount();

        /// <summary>
        /// Get grain description by name
        /// </summary>
        /// <returns>
        /// Return description (name, type, id) or null if not found
        /// </returns>
        Task<GrainDetails?> GetGrainByName(string name);

        /// <summary>
        /// Get grain description by name
        /// </summary>
        /// <returns>
        /// Return description (name, type, id) or null if not found
        /// </returns>
        Task<GrainDetails?> GetGrainById(Guid id);

        // tod: use filtering and paging
        Task<IReadOnlyCollection<GrainDetails>> GetGrains(string? filterText = null);
    }
}
