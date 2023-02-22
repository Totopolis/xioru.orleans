using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Runtime;
using Orleans.Streams;
using Xioru.Grain.Contracts;
using Xioru.Grain.Contracts.ClusterRegistry;
using Xioru.Grain.Contracts.Messages;
using Xioru.Grain.Contracts.Project;
using Xioru.Grain.Contracts.Project.Events;
using Xioru.Grain.Contracts.ProjectRegistry;

namespace Xioru.Grain.Project;

public class ProjectGrain : Orleans.Grain, IProjectGrain
{
    private IAsyncStream<GrainEvent> _projectRepositoryStream = default!;
    private IAsyncStream<GrainEvent> _clusterRepositoryStream = default!;

    protected readonly IPersistentState<ProjectState> _state;
    protected readonly ILogger<ProjectGrain> _log;
    protected readonly IMapper _mapper;
    protected readonly IGrainFactory _grainFactory;

    protected ProjectState State => _state.State;

    public ProjectGrain(
        [PersistentState("State", GrainConstants.StateStorageName)] IPersistentState<ProjectState> state,
        IServiceProvider services)
    {
        _state = state;
        _log = services.GetRequiredService<ILogger<ProjectGrain>>();
        _mapper = services.GetRequiredService<IMapper>();
        _grainFactory = services.GetRequiredService<IGrainFactory>();
    }

    public async Task Create(CreateProjectCommand createCommand)
    {
        var projectId = this.GetPrimaryKey();

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

        var clusterRegistry = _grainFactory
            .GetGrain<IClusterRegistryWriterGrain>(Guid.Empty);
        if (!await clusterRegistry.OnProjectCreatedAsync(projectId, createCommand.Name))
        {
            throw new Exception("Project name already exists");
        }
        await _grainFactory.GetGrain<IProjectRegistryWriterGrain>(projectId)
            .OnGrainCreated(createCommand.Name, projectId, this.GetType().FullName!);

        // 2. Save state
        State.Name = createCommand.Name;
        State.DisplayName = createCommand.DisplayName;
        State.Description = createCommand.Description;
        State.CreatedUtc= DateTime.UtcNow;

        await _state.WriteStateAsync();

        // 3. Event sourcing
        await EmitEvent(new ProjectCreatedEvent(State.CreatedUtc));

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
        await EmitEvent(new ProjectDeletedEvent());

        _log.LogInformation($"Project {projectName}");
    }

    protected async Task EmitEvent<T_EVENT>(
        T_EVENT grainEvent)
        where T_EVENT : GrainEvent
    {
        var grainId = this.GetPrimaryKey();

        grainEvent!.Metadata = new GrainEventMetadata
        {
            ProjectId = grainId,
            GrainType = typeof(ProjectGrain).Name,
            GrainId = grainId,
            GrainName = State.Name
        };

        // 2. Emit to project stream
        if (_projectRepositoryStream == null)
        {
            var _streamProvider = this.GetStreamProvider(GrainConstants.StreamProviderName);

            _projectRepositoryStream = _streamProvider.GetStream<GrainEvent>(StreamId.Create(
                ns: GrainConstants.ProjectRepositoryStreamNamespace,
                key: grainId));
        }

        await _projectRepositoryStream.OnNextAsync(grainEvent);

        // 3. Emit to global cluster stream
        if (_clusterRepositoryStream == null)
        {
            var _streamProvider = this.GetStreamProvider(GrainConstants.StreamProviderName);

            _clusterRepositoryStream = _streamProvider.GetStream<GrainEvent>(StreamId.Create(
                ns: GrainConstants.ClusterRepositoryStreamNamespace,
                key: GrainConstants.ClusterStreamId));
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
