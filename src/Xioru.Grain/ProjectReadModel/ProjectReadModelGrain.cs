using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Orleans;
using Orleans.Streams;
using Orleans.Streams.Core;
using Xioru.Grain.Contracts;
using Xioru.Grain.Contracts.Messages;
using Xioru.Grain.Contracts.Project.Events;
using Xioru.Grain.Contracts.ProjectReadModel;

namespace Xioru.Grain.ProjectReadModel;

// Single instance for application (activate by hardcoded clusterId in domain.dll)
[ImplicitStreamSubscription(GrainConstants.ClusterRepositoryStreamNamespace)]
public class ProjectReadModelGrain :
    Orleans.Grain,
    IProjectReadModelGrain,
    IStreamSubscriptionObserver,
    IAsyncObserver<GrainEvent>
{
    public const string ClusterProjectCollection = "ProjectReadModel";

    private readonly IMongoDatabase _database;
    private readonly IMongoCollection<ProjectDocument> _collection1;
    private readonly ILogger<ProjectReadModelGrain> _log;

    public ProjectReadModelGrain(
        IMongoDatabase database,
        ILogger<ProjectReadModelGrain> log)
    {
        _database = database;
        _log = log;

        _collection1 = _database
            .GetCollection<ProjectDocument>(ClusterProjectCollection);
    }

    public async Task<ProjectDescription?> GetProjectById(Guid projectId)
    {
        var projectCursor = await _collection1.FindAsync(x => x.ProjectId == projectId);
        var project = await projectCursor.FirstOrDefaultAsync();

        ProjectDescription? result = project == null ?
            default :
            new ProjectDescription(
                Id: project.ProjectId,
                Name: project.ProjectName);

        return result;
    }

    public async Task<ProjectDescription?> GetProjectByName(string projectName)
    {
        var projectCursor = await _collection1.FindAsync(x => x.ProjectName == projectName);
        var project = await projectCursor.FirstOrDefaultAsync();

        ProjectDescription? result = project == null ?
            default :
            new ProjectDescription(
                Id: project.ProjectId,
                Name: projectName);

        return result;
    }

    public async Task<ProjectDescription[]> GetProjectsByFilter(string projectNameFilter)
    {
        var allProjects = await _collection1.Find(_ => true).ToListAsync();

        var result = new ProjectDescription[0];

        if (allProjects != null)
        {
            result = allProjects
                .Select(x => new ProjectDescription(
                    Id: x.ProjectId,
                    Name: x.ProjectName))
                .ToArray();

            if (!string.IsNullOrWhiteSpace(projectNameFilter))
            {
                result = result
                    .Where(x => x.Name.Contains(projectNameFilter))
                    .ToArray();
            }
        }

        return result;
    }

    public Task OnCompletedAsync() => Task.CompletedTask;

    public Task OnErrorAsync(Exception ex)
    {
        _log.LogError(ex, "Error at consume stream in ProjectReadModel");
        return Task.CompletedTask;
    }

    public async Task OnNextAsync(GrainEvent grainEvent, StreamSequenceToken? token)
    {
        switch (grainEvent)
        {
            case ProjectCreatedEvent:
                var docToInsert = new ProjectDocument
                {
                    ProjectId = grainEvent.Metadata!.GrainId,
                    ProjectName = grainEvent.Metadata!.GrainName
                };

                await _collection1.InsertOneAsync(docToInsert);
                break;
            case ProjectUpdatedEvent:
                var docToUpdate = new ProjectDocument
                {
                    ProjectId = grainEvent.Metadata!.GrainId,
                    ProjectName = grainEvent.Metadata.GrainName
                };

                await _collection1.ReplaceOneAsync(
                    x => x.ProjectId == grainEvent.Metadata.GrainId,
                    docToUpdate);
                break;
            case ProjectDeletedEvent:
                await _collection1
                    .DeleteOneAsync(x => x.ProjectId == grainEvent.Metadata!.GrainId);
                break;
        }
    }

    public async Task OnSubscribed(IStreamSubscriptionHandleFactory handleFactory)
    {
        var handle = handleFactory.Create<GrainEvent>();
        await handle.ResumeAsync(this);
    }
}
