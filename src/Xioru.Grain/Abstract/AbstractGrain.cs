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

namespace Xioru.Grain.AbstractGrain
{
    public abstract class AbstractGrain<
        T,
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
        private IAsyncStream<GrainMessage> _projectRepositoryStream = default!;
        private IAsyncStream<GrainMessage> _clusterRepositoryStream = default!;

        protected readonly IPersistentState<T_STATE> _state;
        protected readonly ILogger<T> _log;
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
            _log = services.GetRequiredService<ILogger<T>>();
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
            await OnDeleteEmitEvent();

            // 2. Delete state after emit event!!!
            await _state.ClearStateAsync();

            _log.LogInformation($"Grain {objName} deleted in project {projectId}");

            // 3. User logic
            await OnDeleted();
        }

        protected abstract Task OnDeleteEmitEvent();
        protected abstract Task OnDeleted();

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

        protected async Task EmitEvent(
            GrainMessage.MessageKind kind = GrainMessage.MessageKind.Other,
            object? @event = null)
        {
            // 1. Prepare event
            var json = @event == null ? string.Empty : JsonSerializer.Serialize(@event);

            var grainEvent = new GrainMessage
            {
                ProjectId = State.ProjectId,
                BaseGrainType = typeof(T).BaseType!.Name,
                GrainType = typeof(T).Name,
                GrainId = this.GetPrimaryKey(),
                GrainName = State.Name,
                Kind = kind,
                CreatedUtc = DateTime.UtcNow,
                BaseEventType = @event == null ? string.Empty : @event.GetType().BaseType!.Name,
                EventType = @event == null ? string.Empty : @event.GetType().Name,
                EventBody = json
            };

            // 2. Emit to project stream
            if (_projectRepositoryStream == null)
            {
                var _streamProvider = GetStreamProvider("SMSProvider");

                _projectRepositoryStream = _streamProvider.GetStream<GrainMessage>(
                    streamId: State.ProjectId,
                    streamNamespace: GrainConstants.ProjectRepositoryStreamNamespace);
            }

            await _projectRepositoryStream.OnNextAsync(grainEvent);

            // 3. Emit to global cluster stream
            if (_clusterRepositoryStream == null)
            {
                var _streamProvider = GetStreamProvider("SMSProvider");

                _clusterRepositoryStream = _streamProvider.GetStream<GrainMessage>(
                    streamId: GrainConstants.ClusterStreamId,
                    streamNamespace: GrainConstants.ClusterRepositoryStreamNamespace);
            }

            await _clusterRepositoryStream.OnNextAsync(grainEvent);
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
