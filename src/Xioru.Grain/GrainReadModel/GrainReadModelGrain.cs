using AutoMapper;
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
using Xioru.Grain.GrainReadModel.State;

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
        private readonly IReadModelEventHandler _eventHandler;
        private readonly IMapper _mapper;
        private readonly ILogger<GrainReadModelGrain> _log;
        private IMongoCollection<GrainDetailsDocument> _grainCollection = default!;

        public GrainReadModelGrain(
            IMongoDatabase database,
            IReadModelEventHandler eventHandler,
            IMapper mapper,
            ILogger<GrainReadModelGrain> log)
        {
            _database = database;
            _mapper = mapper;
            _eventHandler = eventHandler;
            _log = log;
        }

        public override async Task OnActivateAsync()
        {
            // activated only prjId instances by implicit streaming
            var dbNamePrefix = this.GetPrimaryKey().ToString("N");
            
            var collectionName = $"{dbNamePrefix}-{GrainReadModelCollectionName}";
            _eventHandler.SetCollectionName(collectionName);
            _grainCollection = _database.GetCollection<GrainDetailsDocument>(collectionName);

            await base.OnActivateAsync();
        }

        public async Task<long> GrainsCount()
        {
            return await _grainCollection.CountDocumentsAsync(x => true);
        }

        public async Task<GrainDetails?> GetGrainByName(string name)
        {
            var grainCursor = await _grainCollection.FindAsync(x => x.GrainName == name);
            var grain = await grainCursor.FirstOrDefaultAsync();

            return grain == null ? null :
                _mapper.Map<GrainDetails>(grain);
        }

        public async Task<GrainDetails?> GetGrainById(Guid id)
        {
            var grainCursor = await _grainCollection.FindAsync(x => x.GrainId == id);
            var grain = await grainCursor.FirstOrDefaultAsync();

            return grain == null ? null :
                _mapper.Map<GrainDetails>(grain);
        }

        public async Task<IReadOnlyCollection<GrainDetails>> GetGrains(string? filterText = null)
        {
            var filter = filterText == null
                ? Builders<GrainDetailsDocument>.Filter.Empty
                : Builders<GrainDetailsDocument>.Filter.Or(
                    Builders<GrainDetailsDocument>.Filter.Regex(x => x.GrainName, 
                        new BsonRegularExpression(
                        new Regex(filterText, RegexOptions.IgnoreCase))),
                    Builders<GrainDetailsDocument>.Filter.Regex(x => x.GrainType,
                        new BsonRegularExpression(
                        new Regex(filterText, RegexOptions.IgnoreCase))));
            var list = await _grainCollection.Find(filter).ToListAsync();

            var result = list.Count == 0 ? Array.Empty<GrainDetails>() :
                list.Select(_mapper.Map<GrainDetails>)
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
