using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Orleans;
using Orleans.Streams;
using Orleans.Streams.Core;
using Xioru.Grain.Contracts;
using Xioru.Grain.Contracts.ProjectReadModel;
using Xioru.Grain.Project;

namespace Xioru.Grain.ProjectReadModel
{
    // Single instance for application (activate by hardcoded clusterId in domain.dll)
    [ImplicitStreamSubscription(GrainConstants.ClusterRepositoryStreamNamespace)]
    public class ProjectReadModelGrain :
        Orleans.Grain,
        IProjectReadModelGrain,
        IStreamSubscriptionObserver,
        IAsyncObserver<GrainMessage>
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

        public async Task<ProjectDescription?> GetProjectByName(string projectName)
        {
            var projectCursor = await _collection1.FindAsync(x => x.ProjectName == projectName);
            var project = await projectCursor.FirstOrDefaultAsync();

            ProjectDescription? result = project == null ?
                default :
                new ProjectDescription
                {
                    Id = project.ProjectId,
                    Name = projectName
                };

            return result;
        }

        public async Task<ProjectDescription[]> GetProjectsByFilter(string projectNameFilter)
        {
            var allProjects = await _collection1.Find(_ => true).ToListAsync();

            var result = new ProjectDescription[0];

            if (allProjects != null)
            {
                result = allProjects
                    .Select(x => new ProjectDescription
                    {
                        Id = x.ProjectId,
                        Name = x.ProjectName
                    })
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

        public Task OnCompletedAsync()
        {
            return Task.CompletedTask;
        }

        public Task OnErrorAsync(Exception ex)
        {
            _log.LogError(ex, "Error at consume stream in ProjectReadModel");
            return Task.CompletedTask;
        }

        public async Task OnNextAsync(GrainMessage item, StreamSequenceToken token = default!)
        {
            if (item.GrainType != typeof(ProjectGrain).Name)
            {
                return;
            }

            switch (item.Kind)
            {
                case GrainMessage.MessageKind.Create:
                    var docToInsert = new ProjectDocument
                    {
                        ProjectId = item.GrainId,
                        ProjectName = item.GrainName
                    };

                    await _collection1.InsertOneAsync(docToInsert);
                    break;
                case GrainMessage.MessageKind.Update:
                    var docToUpdate = new ProjectDocument
                    {
                        ProjectId = item.GrainId,
                        ProjectName = item.GrainName
                    };

                    await _collection1.ReplaceOneAsync(
                        x => x.ProjectId == item.GrainId,
                        docToUpdate);
                    break;
                case GrainMessage.MessageKind.Delete:
                    await _collection1
                        .DeleteOneAsync(x => x.ProjectId == item.GrainId);
                    break;
                case GrainMessage.MessageKind.Other:
                    break;
                default:
                    break;
            }
        }

        public async Task OnSubscribed(IStreamSubscriptionHandleFactory handleFactory)
        {
            var handle = handleFactory.Create<GrainMessage>();
            await handle.ResumeAsync(this);
        }
    }
}
