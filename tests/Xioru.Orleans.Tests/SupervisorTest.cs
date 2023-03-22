using System;
using System.Threading.Tasks;
using Xioru.Grain.Contracts;
using Xioru.Grain.Contracts.ClusterRegistry;
using Xioru.Orleans.Tests.Common;
using Xioru.Orleans.Tests.VirtualMessenger;
using Xunit;

namespace Xioru.Orleans.Tests;

public class SupervisorTest : AbstractTest
{
    [Fact]
    public async Task UnameCommand_ExecuteAsSupervisor_SuccessReturnsVersion100()
    {
        var messenger = _factory.GetGrain<IVirtualMessengerGrain>(Guid.Empty);

        var result = await messenger.ExecuteSupervisorCommand("/uname");

        Assert.True(result.IsSuccess, result.Message);
        Assert.Contains("1.0.0", result.Message);
    }

    [Fact]
    public async Task StartCommand_ExecuteAsSupervisor_ClusterRegistryUpdated()
    {
        var messenger = _factory.GetGrain<IVirtualMessengerGrain>(Guid.Empty);

        var result = await messenger.ExecuteSupervisorCommand("/start myproject");
        Assert.True(result.IsSuccess, result.Message);

        var clusterRegistry = _factory.GetGrain<IClusterRegistryGrain>(GrainConstants.ClusterStreamId);
        var projectId = await clusterRegistry.GetProjectIdByNameOrDefaultAsync("myproject");

        Assert.NotNull(projectId);
    }

    [Fact]
    public async Task SpwCommand_WhenProjectCreated_ReturnsProjectInList()
    {
        var messenger = _factory.GetGrain<IVirtualMessengerGrain>(Guid.Empty);

        var result = await messenger.ExecuteSupervisorCommand("/start myproject");
        Assert.True(result.IsSuccess, result.Message);

        result = await messenger.ExecuteSupervisorCommand("/s-pwd");
        Assert.True(result.IsSuccess, result.Message);
        Assert.Contains("myproject", result.Message);
    }

    [Fact]
    public async Task HelpCommand_ExecuteAsSupervisor_ReturnsTwoParts()
    {
        var messenger = _factory.GetGrain<IVirtualMessengerGrain>(Guid.Empty);

        var result = await messenger.ExecuteSupervisorCommand("/help");
        Assert.True(result.IsSuccess, result.Message);
        Assert.Contains("Common commands", result.Message);
        Assert.Contains("Project commands", result.Message);

        result = await messenger.ExecuteSupervisorCommand("/help start");
        Assert.True(result.IsSuccess, result.Message);
    }

    [Fact]
    public async Task SpecificHelpCommand_ExecuteAsSupervisor_ReturnsStandartOutput()
    {
        var messenger = _factory.GetGrain<IVirtualMessengerGrain>(Guid.Empty);

        var result = await messenger.ExecuteSupervisorCommand("/help start");
        Assert.True(result.IsSuccess, result.Message);
        Assert.Contains("Description", result.Message);
        Assert.Contains("Usage", result.Message);
        Assert.Contains("Arguments", result.Message);
    }
}
