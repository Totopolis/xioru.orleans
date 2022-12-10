using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Orleans.Streams;
using Orleans.Streams.Core;
using Xioru.Grain.Contracts;
using Xioru.Grain.Contracts.ApiKey.Events;
using Xioru.Grain.Contracts.ApiKeyReadModel;
using Xioru.Grain.Contracts.Messages;

namespace Xioru.Grain.ApiKeyReadModel;

// Single instance for application
[ImplicitStreamSubscription(GrainConstants.ClusterRepositoryStreamNamespace)]
public class ApiKeyReadModelGrain :
    Orleans.Grain,
    IApiKeyReadModelGrain,
    IStreamSubscriptionObserver,
    IAsyncObserver<GrainEvent>
{
    public const string ClusterApiKeyCollection = "ApiKeyReadModel";

    private readonly IMongoDatabase _database;
    private readonly IMemoryCache _memoryCache;
    private readonly IMongoCollection<ApiKeyDocument> _apiKeyCollection;
    private readonly ILogger<ApiKeyReadModelGrain> _log;

    public ApiKeyReadModelGrain(
        IMongoDatabase database,
        IMemoryCache memoryCache,
        ILogger<ApiKeyReadModelGrain> log)
    {
        _database = database;
        _memoryCache = memoryCache;
        _log = log;

        _apiKeyCollection = _database
            .GetCollection<ApiKeyDocument>(ClusterApiKeyCollection);
    }

    public async Task<ApiKeyDescription?> TryGetApiKey(Guid apiKey)
    {
        var cachedValue = await _memoryCache.GetOrCreateAsync(
            apiKey,
            async cacheEntry =>
            {
                // cacheEntry.SlidingExpiration = TimeSpan.FromSeconds(3);
                cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(20);

                var projectCursor = await _apiKeyCollection.FindAsync(x => x.ApiKey == apiKey);
                var record = await projectCursor.FirstOrDefaultAsync();

                return record == null ? null : new ApiKeyDescription
                {
                    ApiKey = apiKey,
                    Created = record.Created,
                    ProjectId = record.ProjectId
                };
            });

        return cachedValue;
    }

    public Task OnCompletedAsync()
    {
        return Task.CompletedTask;
    }

    public Task OnErrorAsync(Exception ex)
    {
        _log.LogError(ex, "Error at consume stream in ApiKeyReadModel");
        return Task.CompletedTask;
    }

    public async Task OnNextAsync(GrainEvent grainEvent, StreamSequenceToken? token)
    {
        switch (grainEvent)
        {
            case ApiKeyCreatedEvent createdEvent:

                var docToInsert = new ApiKeyDocument
                {
                    ApiKey = createdEvent.ApiKey,
                    Created = createdEvent.CreatedUtc,
                    ProjectId = grainEvent.Metadata!.ProjectId,
                    GrainId = grainEvent.Metadata.GrainId
                };

                await _apiKeyCollection.InsertOneAsync(docToInsert);
                break;

            case ApiKeyDeletedEvent:
                await _apiKeyCollection
                    .DeleteOneAsync(x => x.GrainId == grainEvent.Metadata!.GrainId);
                break;

            default:
                break;
        }
    }

    public async Task OnSubscribed(IStreamSubscriptionHandleFactory handleFactory)
    {
        var handle = handleFactory.Create<GrainEvent>();
        await handle.ResumeAsync(this);
    }
}
