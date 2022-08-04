using Orleans;

namespace Xioru.Grain.Contracts.ApiKey
{
    public interface IApiKeyGrain : IGrainWithGuidKey
    {
        Task CreateAsync(CreateApiKeyCommandModel createCommand);

        Task UpdateAsync(UpdateApiKeyCommandModel updateCommand);

        Task DeleteAsync();

        Task<ApiKeyProjection> GetProjection();
    }
}
