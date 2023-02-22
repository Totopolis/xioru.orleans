using Orleans;
using System;
using System.Threading.Channels;
using System.Threading.Tasks;
using Xioru.Grain;
using Xioru.Grain.Contracts;
using Xioru.Grain.Contracts.ClusterRegistry;
using Xioru.Grain.Contracts.GrainReadModel;
using Xioru.Grain.Contracts.Project;
using Xioru.Grain.Contracts.ProjectRegistry;
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

    public AbstractTest(TestsFixture fixture)
    {
        _factory = fixture.Cluster.GrainFactory;

        _projectId = Guid.NewGuid();
        _projectName = $"IntegrationProject_{_projectId.ToString("N")}";
        _project = _factory.GetGrain<IProjectGrain>(_projectId);

        _channelId = Guid.NewGuid();
        _channelName = _channelId.ToString("N");
        _channel = _factory.GetGrain<IChannelGrain>(_channelId);
    }

    protected async Task PrepareAsync()
    {
        await _project.Create(new CreateProjectCommand(
            Name: _projectName,
            DisplayName: _projectName,
            Description: string.Empty));

        await _channel.CreateAsync(new CreateChannelCommandModel(
            ProjectId: _projectId,
            Name: _channelName,
            DisplayName: _channelName,
            Description: string.Empty,
            Tags: Array.Empty<string>(),
            //
            MessengerType: MessengerType.Virtual,
            ChatId: Guid.NewGuid().ToString("N")));
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

        var success = await _factory.CheckGrainExistsInProjectAsync(_projectId, fooId);

        if (!success)
        {
            throw new Exception("Foo not created");
        }

        return foo;
    }
}
