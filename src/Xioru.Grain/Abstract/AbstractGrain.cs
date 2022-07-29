using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Runtime;
using Orleans.Streams;
using System.Text.Json;
using Xioru.Grain.Contracts;
using Xioru.Grain.Contracts.AbstractGrain;
using Xioru.Grain.Contracts.Messages;

namespace Xioru.Grain.AbstractGrain
{
    public abstract class AbstractGrain<
        T_STATE,
        T_CREATE_COMMAND,
        T_UPDATE_COMMAND,
        T_PROJECTION> :
        Orleans.Grain, IGrainWithGuidKey
        where T_STATE : AbstractGrainState
        where T_CREATE_COMMAND : notnull, CreateAbstractGrainCommand
        where T_UPDATE_COMMAND : notnull, UpdateAbstractGrainCommand
        where T_PROJECTION : AbstractGrainProjection
    {
        private IAsyncStream<GrainEvent> _projectRepositoryStream = default!;
        private IAsyncStream<GrainEvent> _clusterRepositoryStream = default!;

        protected readonly IPersistentState<T_STATE> _state;
        protected readonly ILogger _log;
        protected readonly IMapper _mapper;
        protected readonly IGrainFactory _grainFactory;
        protected readonly IValidator<T_CREATE_COMMAND> _createValidator;
        protected readonly IValidator<T_UPDATE_COMMAND> _updateValidator;

        protected T_STATE State => _state.State;

        public AbstractGrain(
            IPersistentState<T_STATE> state,
            IServiceProvider services)
        {
            _state = state;
            var loggerFactory = services.GetRequiredService<ILoggerFactory>();
            _log = loggerFactory.CreateLogger(this.GetType());
            _mapper = services.GetRequiredService<IMapper>();
            _grainFactory = services.GetRequiredService<IGrainFactory>();

            _createValidator = services.GetRequiredService<IValidator<T_CREATE_COMMAND>>();
            _updateValidator = services.GetRequiredService<IValidator<T_UPDATE_COMMAND>>();
        }

        public async Task Create(T_CREATE_COMMAND createCommand)
        {
            // 0. Check state
            if (_state.RecordExists)
            {
                throw new Exception("Grain already exists");
            }

            var vcontext = new ValidationContext<T_CREATE_COMMAND>(createCommand);
            vcontext.RootContextData["grain"] = this;
            var vr = await _createValidator.ValidateAsync(vcontext);

            if (!vr.IsValid)
            {
                var msg = string.Join(';', vr.Errors.Select(x => x.ErrorMessage));
                throw new Exception(msg);
            }
 
            // 2. Save state
            // TODO: use mapping
            State.Name = createCommand.Name;
            State.ProjectId = createCommand.ProjectId;
            State.DisplayName = createCommand.DisplayName ?? createCommand.Name;
            State.Description = createCommand.Description;
            State.Tags = createCommand.Tags == null ?
                new string[0].ToList() :
                createCommand.Tags.ToList();

            await OnCreateApplyState(createCommand);

            await _state.WriteStateAsync();

            // 3. Event sourcing
            await OnCreateEmitEvent(createCommand);

            _log.LogInformation($"Grain {createCommand.Name} created in project {createCommand.ProjectId}");

            // 4. User logic
            await OnCreated();
        }

        protected abstract Task OnCreateApplyState(T_CREATE_COMMAND createCommand);
        protected abstract Task OnCreateEmitEvent(T_CREATE_COMMAND createCommand);
        protected abstract Task OnCreated();

        public async Task Delete()
        {
            // 0. Check state
            CheckState();

            var projectId = State.ProjectId;
            var objName = State.Name;

            // 1. Event sourcing
            await EmitDeleteEvent();

            // 2. Delete state after emit event!!!
            await _state.ClearStateAsync();

            _log.LogInformation($"Grain {objName} deleted in project {projectId}");

            // 3. User logic
            await OnDeleted();
        }

        protected abstract Task EmitDeleteEvent();

        protected virtual Task OnDeleted() => Task.CompletedTask;

        public async Task Update(T_UPDATE_COMMAND updateCommand)
        {
            // 0. Check state
            if (!_state.RecordExists)
            {
                throw new Exception("Grain does not exists");
            }

            var vcontext = new ValidationContext<T_UPDATE_COMMAND>(updateCommand);
            vcontext.RootContextData["grain"] = this;
            var vr = await _updateValidator.ValidateAsync(vcontext);

            if (!vr.IsValid)
            {
                var msg = string.Join(';', vr.Errors.Select(x => x.ErrorMessage));
                throw new Exception(msg);
            }

            // 2. Save state
            // TODO: use mapping
            State.DisplayName = updateCommand.DisplayName ?? State.Name;
            State.Description = updateCommand.Description;
            State.Tags = updateCommand.Tags.ToList();

            await OnUpdateApplyState(updateCommand);

            await _state.WriteStateAsync();

            // 3. Event sourcing
            await OnUpdateEmitEvent(updateCommand);

            _log.LogInformation($"Grain {State.Name} updated");

            // 4. User logic
            await OnUpdated();
        }

        protected abstract Task OnUpdateApplyState(T_UPDATE_COMMAND updateCommand);
        protected abstract Task OnUpdateEmitEvent(T_UPDATE_COMMAND updateCommand);
        protected abstract Task OnUpdated();

        protected void CheckState()
        {
            if (!_state.RecordExists)
            {
                throw new Exception("Grain does not exists");
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
            grainEvent.Metadata = new GrainEventMetadata
            {
                ProjectId = State.ProjectId,
                BaseGrainType = this.GetType().BaseType!.Name,
                GrainType = this.GetType().Name,
                GrainId = this.GetPrimaryKey(),
                GrainName = State.Name,
                CreatedUtc = DateTime.UtcNow
            };

            // 2. Emit to project stream
            if (_projectRepositoryStream == null)
            {
                var streamProvider = GetStreamProvider("SMSProvider");

                _projectRepositoryStream = streamProvider.GetStream<GrainEvent>(
                    streamId: State.ProjectId,
                    streamNamespace: GrainConstants.ProjectRepositoryStreamNamespace);
            }

            await _projectRepositoryStream.OnNextAsync(grainEvent!);

            // 3. Emit to global cluster stream
            if (_clusterRepositoryStream == null)
            {
                var streamProvider = GetStreamProvider("SMSProvider");

                _clusterRepositoryStream = streamProvider.GetStream<GrainEvent>(
                    streamId: GrainConstants.ClusterStreamId,
                    streamNamespace: GrainConstants.ClusterRepositoryStreamNamespace);
            }

            await _clusterRepositoryStream.OnNextAsync(grainEvent!);
        }

        public Task<T_PROJECTION> GetProjection()
        {
            var projection = _mapper.Map<T_PROJECTION>(
                State,
                opt => opt.Items["Grain"] = this);

            return Task.FromResult(projection);
        }
    }
}
