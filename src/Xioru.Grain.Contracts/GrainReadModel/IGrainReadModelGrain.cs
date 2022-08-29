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
        Task<GrainDetails?> GetGrainDetailsByName(string name);

        /// <summary>
        /// Get grain description by name
        /// </summary>
        /// <returns>
        /// Return description (name, type, id) or null if not found
        /// </returns>
        Task<GrainDetails?> GetGrainById(Guid id);

        // tod: use filtering and paging
        Task<IReadOnlyCollection<GrainDetails>> GetGrains(string? filterText = null);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        Task<T?> GetGrainByNameOrDefault<T>(string name) where T : class, IGrainWithGuidKey;

        /// <summary>
        /// Get grain by id if it exists.
        /// </summary>
        Task<T?> GetGrainByIdOrDefault<T>(Guid id) where T : class, IGrainWithGuidKey;

        /// <summary>
        /// Get grain description by name if it exists and implements T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        Task<GrainDetails?> GetGrainDetailsByNameAndInterface<T>(string name) where T : class, IGrainWithGuidKey;

        /// <summary>
        /// Get grain description by id if it exists and implements T.
        /// </summary>
        Task<GrainDetails?> GetGrainDetailsByIdAndInterface<T>(Guid id) where T : class, IGrainWithGuidKey;
    }
}
