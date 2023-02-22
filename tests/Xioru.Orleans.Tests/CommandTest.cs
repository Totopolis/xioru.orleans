using System;
using System.CommandLine;
using System.Linq;
using System.Threading.Tasks;
using Xioru.Grain;
using Xioru.Grain.Contracts.ProjectRegistry;
using Xioru.Orleans.Tests.Common;
using Xioru.Orleans.Tests.Contracts;
using Xunit;

namespace Xioru.Orleans.Tests;

[Collection(TestsCollection.Name)]
public class CommandTest : AbstractTest
{
    public CommandTest(TestsFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public async Task ListCommand_ReadModelContainsGrains_ReturnsTheGrains()
    {
        await PrepareAsync();
        var foo = await InternalCreateFoo("Foo1");
        await Task.Delay(300);

        var result = await _channel.ExecuteCommand("/list");
        var text = result.Message.ToString();

        Assert.True(result.IsSuccess, result.Message);
        Assert.False(string.IsNullOrEmpty(text));

        Assert.Contains(_projectName, text);
        Assert.Contains("Project", text);

        Assert.Contains(_channelName, text);
        Assert.Contains("Channel", text);

        Assert.Contains("Foo1", text);
        Assert.Contains("Foo", text);
    }

    [Fact]
    public async Task CheckDetailsCommand()
    {
        await PrepareAsync();
        var foo = await InternalCreateFoo("Foo1");

        var result = await _channel.ExecuteCommand("/details Foo1");
        var text = result.Message.ToString();

        Assert.True(result.IsSuccess);
        Assert.False(string.IsNullOrEmpty(text));

        Assert.Contains("Foo", text);
    }

    [Fact]
    public async Task CreateFooCommand()
    {
        await PrepareAsync();

        var result = await _channel.ExecuteCommand("/upsert foo1 data=123");
        Assert.True(result.IsSuccess, result.Message);

        await _factory.CheckGrainExistsInProjectAsync(_projectId, "foo1");

        var grain = await _factory.GetGrainFromProjectAsync<IFooGrain>(_projectId, "foo1");

        var projection = await grain!.GetProjection();
        Assert.Equal("123", projection.FooData);
        Assert.Equal("nodata", projection.FooMeta);
    }

    [Fact]
    public async Task UpsertFooCommand()
    {
        await PrepareAsync();

        await _channel.ExecuteCommand("/upsert foo1 data=123");
        await _factory.CheckGrainExistsInProjectAsync(_projectId, "foo1");

        var result = await _channel.ExecuteCommand("/upsert foo1 data=666 meta=111");
        Assert.True(result.IsSuccess, result.Message);

        await Task.Delay(500);

        var grain = await _factory.GetGrainFromProjectAsync<IFooGrain>(_projectId, "foo1");
        var projection = await grain.GetProjection();
        Assert.Equal("666", projection.FooData);
        Assert.Equal("111", projection.FooMeta);
    }

    [Fact]
    public void CommandLineParse_CustomCommand_GetsArgumentsAndOptionsCorrectly()
    {
        var xxx = "hello tada key=1 val=\"2 3\" val=4";

        var whatArgument = new Argument<string>("what");
        var meArgument = new Argument<string>("me", () => "pusto");
        var keyOption = new Option<int>("key");
        var valOption = new Option<string>("val")
        {
            AllowMultipleArgumentsPerToken = true
        };

        var cmd = new RootCommand
        {
            new Command("hello")
            {
                whatArgument,
                meArgument,
                keyOption,
                valOption,
                new Option<string?>(new[] { "--greeting", "-g" }, "The greeting to use."),
                new Option<string?>(new[] { "--verbose", "-v" }, "Show the deets."),
            }
        };

        var result = cmd.Parse(xxx);
        Assert.Empty(result.Errors);

        var what = result.GetValueForArgument(whatArgument);
        Assert.Equal("tada", what);

        var me = result.GetValueForArgument(meArgument);
        Assert.Equal("pusto", me);

        var key = result.GetValueForOption(keyOption);
        Assert.Equal(1, key);

        var valValues = result.CommandResult.Children
            .ToArray()[2].Tokens
            .Select(x => x.Value)
            .ToArray();
    }
}
