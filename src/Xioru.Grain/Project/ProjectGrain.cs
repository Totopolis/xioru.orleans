using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Runtime;
using Orleans.Streams;
using System.Text.Json;
using Xioru.Grain.Contracts;
using Xioru.Grain.Contracts.Project;
using Xioru.Grain.Contracts.ProjectReadModel;

namespace Xioru.Grain.Project
{
    public class ProjectGrain : Orleans.Grain, IProjectGrain
    {
        private IAsyncStream<GrainMessage> _projectRepositoryStream = default!;
        private IAsyncStream<GrainMessage> _clusterRepositoryStream = default!;

        protected readonly IPersistentState<ProjectState> _state;
        protected readonly ILogger<ProjectGrain> _log;
        protected readonly IMapper _mapper;
        protected readonly IGrainFactory _grainFactory;

        protected ProjectState State => _state.State;

        public ProjectGrain(
            [PersistentState("state", "iotStore")] IPersistentState<ProjectState> state,
            IServiceProvider services)
        {
            _state = state;
            _log = services.GetRequiredService<ILogger<ProjectGrain>>();
            _mapper = services.GetRequiredService<IMapper>();
            _grainFactory = services.GetRequiredService<IGrainFactory>();
        }

        public async Task Create(CreateProjectCommand createCommand)
        {
            var grainId = this.GetPrimaryKey();

            // 0. Check state
            if (_state.RecordExists)
            {
                throw new Exception("Grain already exists");
            }

            // 1. Validate names
            if (string.IsNullOrWhiteSpace(createCommand.Name) ||
                string.IsNullOrWhiteSpace(createCommand.DisplayName))
            {
                throw new Exception("Empty string in name or project");
            }

            // Check projectId
            /*if (createCommand.ProjectId != grainId)
            {
                throw new Exception("Bad projectId value");
            }*/

            // Check project name already exists
            var projectReadModel = _grainFactory
                .GetGrain<IProjectReadModelGrain>(GrainConstants.ClusterStreamId);
            if (await projectReadModel.GetProjectByName(createCommand.Name) != default)
            {
                throw new Exception("Project name already exists");
            }

            // 2. Save state
            State.Name = createCommand.Name;
            State.DisplayName = createCommand.DisplayName;
            State.Description = createCommand.Description;

            await _state.WriteStateAsync();

            // 3. Event sourcing
            await EmitEvent(new ProjectCreatedEvent(), GrainMessage.MessageKind.Create);

            _log.LogInformation($"Project {createCommand.Name} created");
        }

        public async Task Delete()
        {
            // 0. Check state
            if (!_state.RecordExists)
            {
                throw new Exception("Grain does not exists");
            }

            var projectName = State.Name;

            // 1. Delete state
            await _state.ClearStateAsync();

            // 2. Event sourcing
            await EmitEvent(new ProjectDeletedEvent(), GrainMessage.MessageKind.Delete);

            _log.LogInformation($"Project {projectName}");
        }

        protected async Task EmitEvent<T_EVENT>(
            T_EVENT @event,
            GrainMessage.MessageKind kind = GrainMessage.MessageKind.Other)
            where T_EVENT : notnull
        {
            var grainId = this.GetPrimaryKey();

            // 1. Prepare event
            var json = JsonSerializer.Serialize(@event);

            var grainEvent = new GrainMessage
            {
                ProjectId = grainId,
                GrainType = typeof(ProjectGrain).Name,
                GrainId = this.GetPrimaryKey(),
                GrainName = State.Name,
                Kind = kind,
                CreatedUtc = DateTime.UtcNow,
                EventType = typeof(T_EVENT).Name,
                EventBody = json
            };

            // 2. Emit to project stream
            if (_projectRepositoryStream == null)
            {
                var _streamProvider = GetStreamProvider("SMSProvider");

                _projectRepositoryStream = _streamProvider.GetStream<GrainMessage>(
                    streamId: grainId,
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

        public Task<ProjectProjection> GetProjection()
        {
            return Task.FromResult(new ProjectProjection(
                Name: State.Name,
                DisplayName: State.DisplayName,
                Description: State.Description));
        }
    }
}
