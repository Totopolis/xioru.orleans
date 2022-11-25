using Orleans;
using System;
using System.Threading.Tasks;
using Xioru.Grain;
using Xioru.Grain.Contracts;
using Xioru.Grain.Contracts.GrainReadModel;
using Xioru.Grain.Contracts.Project;
using Xioru.Grain.Contracts.ProjectReadModel;
using Xioru.Messaging.Contracts.Channel;
using Xioru.Messaging.Contracts.Messenger;
using Xioru.Orleans.Tests.Contracts;

namespace Xioru.Orleans.Tests.Common;

public abstract class AbstractTest
{
    protected readonly IGrainFactory _factory;

    protected readonly Guid _projectId;
    protected readonly string _projectName;
    protected readonly IProjectGrain _project;

    protected readonly Guid _channelId;
    protected readonly IChannelGrain _channel;
    protected readonly string _channelName;
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
        _channelName = _channelId.ToString("N");
        _channel = _factory.GetGrain<IChannelGrain>(_channelId);
        _grainReadModel = _factory.GetGrain<IGrainReadModelGrain>(_projectId);
    }

    protected async Task PrepareAsync()
    {
        await _project.Create(new CreateProjectCommand(
            Name: _projectName,
            DisplayName: _projectName,
            Description: string.Empty));

        var checkProjectExists = async () =>
        {
            var projectDescription = await _projectReadModel.GetProjectById(_projectId);
            return projectDescription != null;
        };

        if (!await checkProjectExists.CheckTimeoutedAsync())
        {
            throw new Exception("Project not created in cluster");
        }

        var checkProjectReadModel = async () =>
        {
            var projectDescription = await _grainReadModel.GetGrainById(_projectId);
            return projectDescription != null;
        };

        if (!await checkProjectReadModel.CheckTimeoutedAsync())
        {
            throw new Exception("Project not created in readModel");
        }

        await _channel.CreateAsync(new CreateChannelCommandModel(
            ProjectId: _projectId,
            Name: _channelName,
            DisplayName: _channelName,
            Description: string.Empty,
            Tags: Array.Empty<string>(),
            //
            MessengerType: MessengerType.Virtual,
            ChatId: Guid.NewGuid().ToString("N")));

        var checkChannelCreated = async () =>
        {
            var channelDetails = await _grainReadModel.GetGrainById(_channelId);
            return channelDetails != null;
        };

        if (!await checkChannelCreated.CheckTimeoutedAsync())
        {
            throw new Exception("Channel not created");
        }
    }

    protected async Task<IFooGrain> InternalCreateFoo(string name)
    {
        var fooId = Guid.NewGuid();
        var foo = _factory.GetGrain<IFooGrain>(fooId);
        await foo.CreateAsync(new CreateFooCommandModel(
            ProjectId: _projectId,
            Name: name,
            DisplayName: name,
            Description: string.Empty,
            Tags: Array.Empty<string>(),
            FooData: $"Hello {name}",
            FooMeta: $"By {name}"));

        var checkFooCreated = async () =>
        {
            var fooDetails = await _grainReadModel.GetGrainById(fooId);
            return fooDetails != null;
        };

        if (!await checkFooCreated.CheckTimeoutedAsync())
        {
            throw new Exception("Foo not created");
        }

        return foo;
    }
}
