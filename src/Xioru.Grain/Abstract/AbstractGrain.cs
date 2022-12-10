using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Runtime;
using Orleans.Streams;
using Xioru.Grain.Contracts;
using Xioru.Grain.Contracts.AbstractGrain;
using Xioru.Grain.Contracts.Exception;
using Xioru.Grain.Contracts.Messages;

namespace Xioru.Grain.AbstractGrain;

public abstract class AbstractGrain<
    T_STATE,
    T_CREATE_COMMAND,
    T_UPDATE_COMMAND,
    T_PROJECTION> :
    Orleans.Grain, IGrainWithGuidKey
    where T_STATE : AbstractGrainState
    where T_CREATE_COMMAND : notnull, CreateAbstractGrainCommandModel
    where T_UPDATE_COMMAND : notnull, UpdateAbstractGrainCommandModel
    where T_PROJECTION : AbstractGrainProjection
{
    private IAsyncStream<GrainEvent> _projectRepositoryStream = default!;
    private IAsyncStream<GrainEvent> _clusterRepositoryStream = default!;

    protected readonly IPersistentState<T_STATE> _state;
    protected readonly ILogger _log;
    protected readonly IMapper _mapper;
    protected readonly IValidator<T_CREATE_COMMAND> _createValidator;
    protected readonly IValidator<T_UPDATE_COMMAND> _updateValidator;

    protected T_STATE State => _state.State;
    protected virtual bool IsCreated => _state.RecordExists;

    public AbstractGrain(
        IPersistentState<T_STATE> state,
        IServiceProvider services)
    {
        _state = state;
        var loggerFactory = services.GetRequiredService<ILoggerFactory>();
        _log = loggerFactory.CreateLogger(this.GetType());
        _mapper = services.GetRequiredService<IMapper>();

        _createValidator = services.GetRequiredService<IValidator<T_CREATE_COMMAND>>();
        _updateValidator = services.GetRequiredService<IValidator<T_UPDATE_COMMAND>>();
    }

    public virtual async Task CreateAsync(T_CREATE_COMMAND createCommand)
    {
        // 0. Check state
        if (IsCreated)
        {
            throw new Exception("Grain already exists");
        }

        // 1. Validate command
        var vcontext = new ValidationContext<T_CREATE_COMMAND>(createCommand);
        vcontext.RootContextData["state"] = State;
        var vr = await _createValidator.ValidateAsync(vcontext);

        if (!vr.IsValid)
        {
            var msg = string.Join(';', vr.Errors.Select(x => x.ErrorMessage));
            throw new Exception(msg);
        }

        // 2. Update local state and force save
        _mapper.Map(createCommand, State);
        State.Id = this.GetPrimaryKey();
        State.CreatedUtc = DateTime.UtcNow;
        State.UpdatedUtc= DateTime.UtcNow;
        await _state.WriteStateAsync();

        // 3. Event sourcing
        await OnCreateEmitEvent(createCommand);

        _log.LogInformation($"Grain {createCommand.Name} created in project {createCommand.ProjectId}");
    }

    protected abstract Task OnCreateEmitEvent(T_CREATE_COMMAND createCommand);

    public virtual async Task DeleteAsync()
    {
        // 0. Check state
        CheckState();

        // 1. Event sourcing
        await EmitDeleteEvent();
        //TODO: map state to DelEvt?

        var name = State.Name;
        var projectId = State.ProjectId;

        // 2. Delete state after emit event!!!
        await _state.ClearStateAsync();

        _log.LogInformation("Grain {Name} deleted in project {ProjectId}", name, projectId);
    }

    protected abstract Task EmitDeleteEvent();

    public virtual async Task UpdateAsync(T_UPDATE_COMMAND updateCommand)
    {
        // 0. Check state
        CheckState();

        // 1. Validate command and update State
        var vcontext = new ValidationContext<T_UPDATE_COMMAND>(updateCommand);
        vcontext.RootContextData["state"] = State;
        var vr = await _updateValidator.ValidateAsync(vcontext);

        if (!vr.IsValid)
        {
            var msg = string.Join(';', vr.Errors.Select(x => x.ErrorMessage));
            throw new Exception(msg);
        }

        // 2. Update local state and force save
        _mapper.Map(updateCommand, State);
        State.UpdatedUtc = DateTime.UtcNow;
        await _state.WriteStateAsync();

        // 3. Event sourcing
        await OnUpdateEmitEvent(updateCommand);

        _log.LogInformation($"Grain {State.Name} updated");
    }

    protected abstract Task OnUpdateEmitEvent(T_UPDATE_COMMAND updateCommand);

    protected void CheckState()
    {
        if (!IsCreated)
        {
            throw new GrainNotCreatedException();
        }

        if (State.ProjectId == Guid.Empty)
        {
            throw new Exception("Grain without project");
        }
    }

    protected async Task EmitEvent(GrainEvent grainEvent)
    {
        grainEvent = grainEvent ?? throw new ArgumentNullException(nameof(grainEvent));
        // 1. Prepare event
        grainEvent.Metadata = Metadata;

        // 2. Emit to project stream
        if (_projectRepositoryStream == null)
        {
            var streamProvider = this.GetStreamProvider(GrainConstants.StreamProviderName);

            _projectRepositoryStream = streamProvider.GetStream<GrainEvent>(StreamId.Create(
                ns: GrainConstants.ProjectRepositoryStreamNamespace,
                key: State.ProjectId));
        }

        await _projectRepositoryStream.OnNextAsync(grainEvent!);

        // 3. Emit to global cluster stream
        if (_clusterRepositoryStream == null)
        {
            var streamProvider = this.GetStreamProvider(GrainConstants.StreamProviderName);

            _clusterRepositoryStream = streamProvider.GetStream<GrainEvent>(StreamId.Create(
                ns: GrainConstants.ClusterRepositoryStreamNamespace,
                key: GrainConstants.ClusterStreamId));
        }

        await _clusterRepositoryStream.OnNextAsync(grainEvent!);
    }

    public Task<T_PROJECTION> GetProjection()
    {
        CheckState();

        var projection = _mapper.Map<T_PROJECTION>(
            State,
            opt => opt.Items["state"] = State);

        return Task.FromResult(projection);
    }

    protected GrainEventMetadata? Metadata => new GrainEventMetadata
    {
        ProjectId = State.ProjectId,
        GrainType = this.GetType().FullName!,
        GrainId = this.GetPrimaryKey(),
        GrainName = State.Name
    };
}
