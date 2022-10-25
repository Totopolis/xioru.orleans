using Orleans;
using System;
using System.Threading.Tasks;
using Xioru.Orleans.Tests.Common;
using Xioru.Orleans.Tests.VirtualMessenger;
using Xunit;

namespace Xioru.Orleans.Tests;

[Collection(TestsCollection.Name)]
public class SupervisorTest
{
    private readonly IGrainFactory _factory;
    
    public SupervisorTest(TestsFixture fixture)
    {
        _factory = fixture.Cluster.GrainFactory;
    }

    [Fact]
    public async Task CheckUname()
    {
        var messenger = _factory.GetGrain<IVirtualMessengerGrain>(Guid.Empty);

        var result = await messenger.ExecuteSupervisorCommand("/uname");
        Assert.True(result.IsSuccess, result.Message);
        Assert.Contains("1.0.0", result.Message);
    }

    [Fact]
    public async Task StartProject()
    {
        var messenger = _factory.GetGrain<IVirtualMessengerGrain>(Guid.Empty);

        var result = await messenger.ExecuteSupervisorCommand("/start myproject");
        Assert.True(result.IsSuccess, result.Message);

        result = await messenger.ExecuteSupervisorCommand("/s-pwd");
        Assert.True(result.IsSuccess, result.Message);
        Assert.Contains("myproject", result.Message);
    }

    [Fact]
    public async Task HelpCommand()
    {
        var messenger = _factory.GetGrain<IVirtualMessengerGrain>(Guid.Empty);

        var result = await messenger.ExecuteSupervisorCommand("/help");
        Assert.True(result.IsSuccess, result.Message);

        result = await messenger.ExecuteSupervisorCommand("/help start");
        Assert.True(result.IsSuccess, result.Message);
    }
}
