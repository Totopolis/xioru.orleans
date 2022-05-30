using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Orleans;
using Orleans.Streams;
using Orleans.Streams.Core;
using System.Text.Json;
using Xioru.Grain.ApiKey;
using Xioru.Grain.Contracts;
using Xioru.Grain.Contracts.ApiKey;
using Xioru.Grain.Contracts.ApiKeyReadModel;

namespace Xioru.Grain.ApiKeyReadModel
{
    // Single instance for application
    [ImplicitStreamSubscription(GrainConstants.ClusterRepositoryStreamNamespace)]
    public class ApiKeyReadModelGrain :
        Orleans.Grain,
        IApiKeyReadModelGrain,
        IStreamSubscriptionObserver,
        IAsyncObserver<GrainMessage>
    {
        public const string ClusterApiKeyCollection = "ApiKeyReadModel";

        private readonly IMongoDatabase _database;
        private readonly IMemoryCache _memoryCache;
        private readonly IMongoCollection<ApiKeyDocument> _collection1;
        private readonly ILogger<ApiKeyReadModelGrain> _log;

        public ApiKeyReadModelGrain(
            IMongoDatabase database,
            IMemoryCache memoryCache,
            ILogger<ApiKeyReadModelGrain> log)
        {
            _database = database;
            _memoryCache = memoryCache;
            _log = log;

            _collection1 = _database
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

                    var projectCursor = await _collection1.FindAsync(x => x.ApiKey == apiKey);
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

        public async Task OnNextAsync(GrainMessage item, StreamSequenceToken token = default!)
        {
            if (item.GrainType != typeof(ApiKeyGrain).Name)
            {
                return;
            }

            switch (item.Kind)
            {
                case GrainMessage.MessageKind.Create:
                    var createdEvent = JsonSerializer
                        .Deserialize<ApiKeyCreatedEvent>(item.EventBody);

                    if (createdEvent == null)
                    {
                        _log.LogError($"Bad apikey created event, apikeyId={item.GrainId}");
                        break;
                    }

                    var docToInsert = new ApiKeyDocument
                    {
                        ApiKey = createdEvent.ApiKey,
                        Created = createdEvent.Created,
                        ProjectId = item.ProjectId,
                        GrainId = item.GrainId
                    };

                    await _collection1.InsertOneAsync(docToInsert);
                    break;

                case GrainMessage.MessageKind.Delete:
                    await _collection1
                        .DeleteOneAsync(x => x.GrainId == item.GrainId);
                    break;

                case GrainMessage.MessageKind.Other:
                    break;
                default:
                    break;
            }
        }

        public async Task OnSubscribed(IStreamSubscriptionHandleFactory handleFactory)
        {
            var handle = handleFactory.Create<GrainMessage>();
            await handle.ResumeAsync(this);
        }
    }
}
