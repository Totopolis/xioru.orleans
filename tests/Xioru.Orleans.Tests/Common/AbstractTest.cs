using Orleans;
using System;
using System.Threading.Tasks;
using Xioru.Grain.Contracts;
using Xioru.Grain.Contracts.GrainReadModel;
using Xioru.Grain.Contracts.Project;
using Xioru.Grain.Contracts.ProjectReadModel;
using Xioru.Messaging.Contracts.Channel;
using Xioru.Messaging.Contracts.Messenger;

namespace Xioru.Orleans.Tests.Common
{
    public abstract class AbstractTest
    {
        protected readonly IGrainFactory _factory;

        protected readonly Guid _projectId;
        protected readonly string _projectName;
        protected readonly IProjectGrain _project;

        protected readonly Guid _channelId;
        protected readonly IChannelGrain _channel;
        protected readonly IGrainReadModelGrain _grainReadModel;
        protected readonly IProjectReadModelGrain _projectReadModel;

        public AbstractTest(TestsFixture fixture)
        {
            _factory = fixture.Cluster.GrainFactory;

            _projectId = Guid.NewGuid();
            _projectName = $"IntegrationProject_{_projectId.ToString("N")}";
            _project = _factory.GetGrain<IProjectGrain>(_projectId);

            _projectReadModel = _factory.GetGrain<IProjectReadModelGrain>(
                GrainConstants.ClusterStreamId);

            _channelId = Guid.NewGuid();
            _channel = _factory.GetGrain<IChannelGrain>(_channelId);
            _grainReadModel = _factory.GetGrain<IGrainReadModelGrain>(_projectId);
        }

        protected async Task PrepareAsync()
        {
            await _project.Create(new CreateProjectCommand(
                Name: _projectName,
                DisplayName: _projectName,
                Description: String.Empty));
            
            await _channel.Create(new CreateChannelCommand(
                ProjectId: _projectId,
                Name: _channelId.ToString(),
                DisplayName: _channelId.ToString(),
                Description: string.Empty,
                Tags: new string[0],
                //
                MessengerType: MessengerType.Discord,
                MessengerId: Guid.Empty,
                ChatId: Guid.NewGuid().ToString("N")));
        }
    }
}
