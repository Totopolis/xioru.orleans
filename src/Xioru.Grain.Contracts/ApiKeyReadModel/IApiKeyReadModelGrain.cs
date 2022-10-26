using Orleans;

namespace Xioru.Grain.Contracts.ApiKeyReadModel;

public interface IApiKeyReadModelGrain : IGrainWithGuidKey
{
    Task<ApiKeyDescription?> TryGetApiKey(Guid apiKey);
}
