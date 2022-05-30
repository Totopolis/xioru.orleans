using Orleans;

namespace Xioru.Grain.Contracts.ApiKey
{
    public interface IApiKeyGrain : IGrainWithGuidKey
    {
        Task Create(CreateApiKeyCommand createCommand);

        Task Update(UpdateApiKeyCommand updateCommand);

        Task Delete();

        Task<ApiKeyProjection> GetProjection();
    }
}
