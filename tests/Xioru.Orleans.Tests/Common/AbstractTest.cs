﻿using Orleans;
using Orleans.TestingHost;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xioru.Grain.Contracts.Project;
using Xioru.Grain.Contracts.ProjectRegistry;
using Xioru.Messaging.Contracts.Channel;
using Xioru.Messaging.Contracts.Messenger;
using Xioru.Orleans.Tests.Contracts;

namespace Xioru.Orleans.Tests.Common;

public abstract class AbstractTest : IDisposable
{
    private readonly TestCluster _cluster;

    protected readonly IGrainFactory _factory;

    protected readonly Guid _projectId;
    protected readonly string _projectName;
    protected readonly IProjectGrain _project;

    protected readonly Guid _channelId;
    protected readonly IChannelGrain _channel;
    protected readonly string _channelName;

    public AbstractTest()
    {
        _cluster = new TestClusterBuilder(initialSilosCount: 1)
            .AddSiloBuilderConfigurator<HostConfigurator>()
            .AddSiloBuilderConfigurator<SiloConfigurator>()
            .Build();

        _cluster.Deploy();

        _factory = _cluster.GrainFactory;

        _projectId = Guid.NewGuid();
        _projectName = $"IntegrationProject_{_projectId.ToString("N")}";
        _project = _factory.GetGrain<IProjectGrain>(_projectId);

        _channelId = Guid.NewGuid();
        _channelName = _channelId.ToString("N");
        _channel = _factory.GetGrain<IChannelGrain>(_channelId);
    }

    public void Dispose()
    {
        // just kill, no expectations!!
        _cluster.GetActiveSilos()
            .ToList()
            .ForEach(x => _cluster.KillSiloAsync(x).Wait());

        // _cluster.StopAllSilos();
        _cluster.Dispose();
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
