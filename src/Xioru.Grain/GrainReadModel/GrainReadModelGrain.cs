using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using Orleans;
using Orleans.Runtime;
using Orleans.Streams;
using Orleans.Streams.Core;
using System.Text.RegularExpressions;
using Xioru.Grain.Contracts;
using Xioru.Grain.Contracts.GrainReadModel;
using Xioru.Grain.Contracts.Messages;

namespace Xioru.Grain.GrainReadModel
{
    [ImplicitStreamSubscription(GrainConstants.ProjectRepositoryStreamNamespace)]
    public class GrainReadModelGrain :
        Orleans.Grain,
        IGrainReadModelGrain,
        IStreamSubscriptionObserver
    {
        public const string GrainReadModelCollectionName = "GrainReadModel";

        private readonly IMongoDatabase _database;
        private readonly ILogger<GrainReadModelGrain> _log;
        private readonly IReadModelEventHandler _eventHandler;
        private IMongoCollection<GrainInfo> _grainCollection = default!;

        public GrainReadModelGrain(
            IMongoDatabase database,
            IReadModelEventHandler eventHandler,
            ILogger<GrainReadModelGrain> log)
        {
            _database = database;
            _eventHandler = eventHandler;
            _log = log;
        }

        public override async Task OnActivateAsync()
        {
            // activated only prjId instances by implicit streaming
            var dbNamePrefix = this.GetPrimaryKey().ToString("N");
            
            var collectionName = $"{dbNamePrefix}-{GrainReadModelCollectionName}";
            _eventHandler.SetCollectionName(collectionName);
            _grainCollection = _database.GetCollection<GrainInfo>(collectionName);

            await base.OnActivateAsync();
        }

        public async Task<long> GrainsCount()
        {
            return await _grainCollection.CountDocumentsAsync(x => true);
        }

        public async Task<GrainDescription?> GetGrainByName(string name)
        {
            var grainCursor = await _grainCollection.FindAsync(x => x.GrainName == name);
            var grain = await grainCursor.FirstOrDefaultAsync();

            return grain == null ? null :
                new GrainDescription()
                {
                    GrainName = grain.GrainName,
                    GrainType = grain.GrainType,
                    GrainId = grain.GrainId
                };
        }

        public async Task<GrainDescription?> GetGrainById(Guid id)
        {
            var grainCursor = await _grainCollection.FindAsync(x => x.GrainId == id);
            var grain = await grainCursor.FirstOrDefaultAsync();

            return grain == null ? null :
                new GrainDescription()
                {
                    GrainName = grain.GrainName,
                    GrainType = grain.GrainType,
                    GrainId = grain.GrainId
                };
        }

        public async Task<GrainDescription[]> GetGrains(string? filterText = null)
        {
            var filter = filterText == null
                ? Builders<GrainInfo>.Filter.Empty
                : Builders<GrainInfo>.Filter.Or(
                    Builders<GrainInfo>.Filter.Regex(x => x.GrainName, 
                        new BsonRegularExpression(
                        new Regex(filterText, RegexOptions.IgnoreCase))),
                    Builders<GrainInfo>.Filter.Regex(x => x.GrainType,
                        new BsonRegularExpression(
                        new Regex(filterText, RegexOptions.IgnoreCase))));
            var list = await _grainCollection.Find(filter).ToListAsync();

            var result = list.Count == 0 ? new GrainDescription[0] :
                list.Select(x => new GrainDescription()
                {
                    GrainId = x.GrainId,
                    GrainName = x.GrainName,
                    GrainType = x.GrainType,
                })
                .ToArray();

            return result;
        }

        public async Task OnSubscribed(IStreamSubscriptionHandleFactory handleFactory)
        {
            var crHandle = handleFactory.Create<GrainCreatedEvent>();
            await crHandle.ResumeAsync(_eventHandler);

        }
    }
}
