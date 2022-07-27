using AutoMapper;
using MongoDB.Driver;
using Orleans.Streams;
using Xioru.Grain.Contracts.Messages;

namespace Xioru.Grain.GrainReadModel
{
    public interface IReadModelEventHandler :
        IAsyncObserver<GrainCreatedEvent>,
        IAsyncObserver<GrainUpdatedEvent>,
        IAsyncObserver<GrainDeletedEvent>
    { 
        void SetCollectionName(string collectionName);
    }

    public class ReadModelEventHandler : IReadModelEventHandler
    {
        private readonly IMongoDatabase _database;
        private readonly IMapper _mapper;
        private IMongoCollection<GrainInfo>? _grainCollection;

        public ReadModelEventHandler(IMongoDatabase database, IMapper mapper)
        {
            _mapper = mapper;
            _database = database;
        }

        public async Task OnNextAsync(GrainCreatedEvent grainEvent, StreamSequenceToken _)
        {
            var infoModel = _mapper.Map<GrainInfo>(grainEvent);

            await _grainCollection!.InsertOneAsync(infoModel);
        }

        public async Task OnNextAsync(GrainUpdatedEvent grainEvent, StreamSequenceToken _)
        {
            var infoModel = _mapper.Map<GrainInfo>(grainEvent);

            await _grainCollection.ReplaceOneAsync(
                x => x.GrainId == grainEvent.Metadata!.GrainId,
                infoModel);
        }

        public async Task OnNextAsync(GrainDeletedEvent item, StreamSequenceToken _)
        {

            await _grainCollection
                            .DeleteOneAsync(x => x.GrainId == item.Metadata!.GrainId);
        }

        public Task OnCompletedAsync()
        {
            return Task.CompletedTask;
        }

        public Task OnErrorAsync(Exception ex)
        {
            //_log.LogError(ex, "Error at consume stream in ProjectReadModel");
            return Task.CompletedTask;
        }

        public void SetCollectionName(string collectionName)
        {
            _grainCollection = _database.GetCollection<GrainInfo>(collectionName);
        }
    }
}
