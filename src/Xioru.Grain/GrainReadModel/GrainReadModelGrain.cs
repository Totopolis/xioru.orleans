﻿using AutoMapper;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using Orleans;
using Orleans.Streams;
using Orleans.Streams.Core;
using System.Text.RegularExpressions;
using Xioru.Grain.Contracts;
using Xioru.Grain.Contracts.GrainReadModel;
using Xioru.Grain.Contracts.Messages;
using Xioru.Grain.GrainReadModel.State;

namespace Xioru.Grain.GrainReadModel;

[ImplicitStreamSubscription(GrainConstants.ProjectRepositoryStreamNamespace)]
public class GrainReadModelGrain :
    Orleans.Grain,
    IAsyncObserver<GrainEvent>,
    IGrainReadModelGrain,
    IStreamSubscriptionObserver
{
    public const string GrainReadModelCollectionName = "GrainReadModel";

    private readonly IMongoDatabase _database;
    private readonly IMapper _mapper;
    private readonly ILogger<GrainReadModelGrain> _log;
    private IMongoCollection<GrainDetailsDocument> _grainCollection = default!;

    public GrainReadModelGrain(
        IMongoDatabase database,
        IMapper mapper,
        ILogger<GrainReadModelGrain> log)
    {
        _database = database;
        _mapper = mapper;
        _log = log;
    }

    public override async Task OnActivateAsync()
    {
        // activated only prjId instances by implicit streaming
        var dbNamePrefix = this.GetPrimaryKey().ToString("N");

        var collectionName = $"{dbNamePrefix}-{GrainReadModelCollectionName}";
        _grainCollection = _database.GetCollection<GrainDetailsDocument>(collectionName);

        await base.OnActivateAsync();
    }

    public async Task<long> GrainsCount()
    {
        return await _grainCollection.CountDocumentsAsync(x => true);
    }

    public async Task<GrainDetails?> GetGrainDetailsByName(string name)
    {
        var grainDetailsDocument = await GetDocumentByNameOrDefaultAsync(name);

        return grainDetailsDocument == null ? null :
            _mapper.Map<GrainDetails>(grainDetailsDocument);
    }

    public async Task<T?> GetGrainByNameOrDefault<T>(string name) where T : class, IGrainWithGuidKey
    {
        var details = await GetDocumentByNameOrDefaultAsync(name);
        if(details == null)
        {
            return null;
        }

        return GrainFactory.GetGrain<T>(details!.GrainId, details.GrainType);
    }

    public async Task<T?> GetGrainByIdOrDefault<T>(Guid id) where T : class, IGrainWithGuidKey
    {
        var details = await GetDocumentByIdOrDefaultAsync(id);
        if (details is null)
        {
            return null;
        }

        try
        {
            return GrainFactory.GetGrain<T>(details.GrainId, details.GrainType);
        }
        catch (ArgumentException)
        {
            return null;
        }
    }

    public async Task<GrainDetails?> GetGrainDetailsByNameAndInterface<T>(string name) where T : class, IGrainWithGuidKey
    {
        var details = await GetDocumentByNameOrDefaultAsync(name);
        if (details == null)
        {
            return null;
        }

        var grain = GrainFactory.GetGrain<T>(details!.GrainId, details.GrainType);
        if (grain == null)
        {
            return null;
        }

        return _mapper.Map<GrainDetails>(details);
    }

    public async Task<GrainDetails?> GetGrainDetailsByIdAndInterface<T>(Guid id) where T : class, IGrainWithGuidKey
    {
        var details = await GetDocumentByIdOrDefaultAsync(id);
        if (details is null)
        {
            return null;
        }

        var grain = GrainFactory.GetGrain<T>(details.GrainId, details.GrainType);
        return grain == null ? null : _mapper.Map<GrainDetails>(details);
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
        var handle = handleFactory.Create<GrainEvent>();
        await handle.ResumeAsync(this);
    }

    public async Task OnNextAsync(GrainEvent grainEvent, StreamSequenceToken? _ = null)
    {
        switch (grainEvent)
        {
            case GrainCreatedEvent:
                var createModel = _mapper.Map<GrainDetailsDocument>(grainEvent);
                await _grainCollection!.InsertOneAsync(createModel);
                break;
            case GrainUpdatedEvent upd:
                var grainCursor = await _grainCollection.FindAsync(x => x.GrainId == upd.Metadata!.GrainId);
                var oldGrainModel = await grainCursor.FirstOrDefaultAsync();

                var updateModel = _mapper.Map<GrainUpdatedEvent, GrainDetailsDocument>(upd, oldGrainModel);
                await _grainCollection.ReplaceOneAsync(
                    x => x.GrainId == grainEvent.Metadata!.GrainId,
                    updateModel);
                break;
            case GrainDeletedEvent:
                await _grainCollection
                    .DeleteOneAsync(x => x.GrainId == grainEvent.Metadata!.GrainId);
                break;
            default:
                break;
        }
    }

    public Task OnCompletedAsync()
    {
        return Task.CompletedTask;
    }

    public Task OnErrorAsync(Exception ex)
    {
        _log.LogError(ex, "Error at consume stream in GrainReadModel");
        return Task.CompletedTask;
    }

    private async Task<GrainDetailsDocument?> GetDocumentByNameOrDefaultAsync(string grainName)
    {
        var grainCursor = await _grainCollection.FindAsync(x => x.GrainName == grainName);
        return await grainCursor.FirstOrDefaultAsync();
    }

    private async Task<GrainDetailsDocument?> GetDocumentByIdOrDefaultAsync(Guid grainId)
    {
        var grainCursor = await _grainCollection.FindAsync(x => x.GrainId == grainId);
        return await grainCursor.FirstOrDefaultAsync();
    }
}
