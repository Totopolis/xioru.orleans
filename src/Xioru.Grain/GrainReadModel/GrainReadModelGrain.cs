using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Orleans;
using Orleans.Streams;
using Orleans.Streams.Core;
using Xioru.Grain.Contracts;
using Xioru.Grain.Contracts.GrainReadModel;

namespace Xioru.Grain.GrainReadModel
{
    [ImplicitStreamSubscription(GrainConstants.ProjectRepositoryStreamNamespace)]
    public partial class GrainReadModelGrain :
        Orleans.Grain,
        IGrainReadModelGrain,
        IStreamSubscriptionObserver,
        IAsyncObserver<GrainMessage>
    {
        public const string GrainReadModelCollectionName = "GrainReadModel";

        private readonly IMongoDatabase _database;
        private readonly ILogger<GrainReadModelGrain> _log;

        private IMongoCollection<GrainDocument> _grainCollection = default!;

        public GrainReadModelGrain(
            IMongoDatabase database,
            ILogger<GrainReadModelGrain> log)
        {
            _database = database;
            _log = log;
        }

        public override async Task OnActivateAsync()
        {
            // activated only prjId instances by implicit streaming
            var dbNamePrefix = this.GetPrimaryKey().ToString("N");
            
            _grainCollection = _database.GetCollection<GrainDocument>(
                $"{dbNamePrefix}-{GrainReadModelCollectionName}");
            var indexDefenition = Builders<GrainDocument>.IndexKeys
                .Text(d => d.GrainName)
                .Text(d => d.GrainType);
            var indexModel = new CreateIndexModel<GrainDocument>(indexDefenition);

            await _grainCollection.Indexes.CreateOneAsync(indexModel);
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
                ? Builders<GrainDocument>.Filter.Empty
                : Builders<GrainDocument>.Filter.Text(filterText);
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
            var handle = handleFactory.Create<GrainMessage>();
            await handle.ResumeAsync(this);
        }

        public async Task OnNextAsync(GrainMessage item, StreamSequenceToken token = default!)
        {
            try
            {
                switch (item.Kind)
                {
                    case GrainMessage.MessageKind.Create:
                        await OnCreateEvent(item);
                        break;

                    case GrainMessage.MessageKind.Update:
                        await OnUpdateEvent(item);
                        break;

                    case GrainMessage.MessageKind.Delete:
                        await OnDeleteEvent(item);
                        break;

                    case GrainMessage.MessageKind.Other:
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Error at consume stream in ProjectReadModel");
            }
        }

        public Task OnCompletedAsync()
        {
            return Task.CompletedTask;
        }

        public Task OnErrorAsync(Exception ex)
        {
            _log.LogError(ex, "Error at consume stream in ProjectReadModel");
            return Task.CompletedTask;
        }
    }
}
