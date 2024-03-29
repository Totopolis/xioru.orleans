using Orleans;
using System.Threading.Tasks;
using Xioru.Grain.Contracts;
using Xioru.Grain.Contracts.ClusterRegistry;
using Xioru.Grain.Contracts.ProjectRegistry;
using Xioru.Orleans.Tests.Common;
using Xioru.Orleans.Tests.Contracts;
using Xioru.Orleans.Tests.Domain;
using Xunit;

namespace Xioru.Orleans.Tests;

public class BaseTest : AbstractTest
{
    [Fact]
    public async Task CreateProject_InClusterRegistry_ProjectCreated()
    {
        await PrepareAsync();

        var name = await _factory.GetGrain<IClusterRegistryGrain>(GrainConstants.ClusterStreamId)
            .GetProjectNameByIdOrDefaultAsync(_projectId);

        Assert.NotNull(name);
    }

    [Fact]
    public async Task CreateChannel_InExistedProject_ChannelCreated()
    {
        await PrepareAsync();

        var projectId = await _factory.GetGrain<IClusterRegistryGrain>(GrainConstants.ClusterStreamId)
            .GetProjectIdByNameOrDefaultAsync(_projectName);
        Assert.NotNull(projectId);
        var channel = await _factory.GetGrain<IProjectRegistryGrain>(projectId.Value)
            .GetGrainDetailsByName(_channelName);
        Assert.NotNull(channel);
    }

    [Fact]
    public async Task SendMessage_ChannelExists_SentWithNoException()
    {
        await PrepareAsync();

        var channelProjection = await _channel.GetProjection();
        Assert.NotNull(channelProjection);

        await _channel.SendMessage("Hello");
    }

    [Fact]
    public async Task CreateFooGrain_ExistedProject_GrainCreatedInRegistry()
    {
        await PrepareAsync();

        await InternalCreateFoo("Foo");

        var details = await _factory.GetGrain<IProjectRegistryGrain>(_projectId)
            .GetGrainDetailsByName("Foo");
        Assert.NotNull(details);
        Assert.Equal("Foo", details!.GrainName);
        Assert.Equal(typeof(FooGrain).FullName, details!.GrainType);
    }

    [Fact]
    public async Task CreateFoo_CorrectCreation_OnlyFooGrainExists()
    {
        await PrepareAsync();

        await InternalCreateFoo("Foo");

        var details = await _factory.GetGrain<IProjectRegistryGrain>(_projectId)
            .GetGrainDetailsByName("Foo");
        Assert.NotNull(details);

        var id = details.GrainId;
        var projection = await _factory.GetGrain<IFooGrain>(details.GrainId)!
            .GetProjection();
        Assert.NotNull(projection);

        var otherGrainExists = await _factory.CheckGrainExistsInProjectAsync<INotFooGrain>(_projectId, id);
        Assert.False(otherGrainExists);
    }
}

public interface INotFooGrain : IGrainWithGuidKey
{
}
