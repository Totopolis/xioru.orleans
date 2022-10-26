using Orleans;
using System;
using System.CommandLine;
using System.Threading.Tasks;
using Xioru.Messaging.Contracts.Channel;
using Xioru.Messaging.Contracts.Command;
using Xioru.Orleans.Tests.Contracts;

namespace Xioru.Orleans.Tests.Domain;

public class UpsertFooCommand : AbstractChannelCommand
{
    // requred argument
    private readonly Argument<string> _nameArgument = new Argument<string>(
            name: "name",
            description: "foo name");

    // required option
    private readonly Option<string> _dataOption = new Option<string>(
            name: "data",
            description: "foo data");

    // optional option
    private readonly Option<string> _metaOption = new Option<string>(
            name: "meta",
            description: "foo meta",
            getDefaultValue: () => "nodata");

    public UpsertFooCommand(IGrainFactory factory) : base(factory)
    {
    }

    protected override Command Command => new Command(
        "upsert", "upsert foo instance")
    {
        _nameArgument,
        _dataOption,
        _metaOption
    };

    protected override async Task<CommandResult> ExecuteInternal(
        ChannelCommandContext context)
    {
        var name = GetArgumentValue(_nameArgument);
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new CommandSyntaxErrorException("Bad foo name");
        }

        var data = GetOptionValue(_dataOption)!;
        var meta = GetOptionValue(_metaOption)!;

        var details = await _grainReadModel.GetGrainDetailsByName(name);
        if (details == null)
        {
            var grain = _factory.GetGrain<IFooGrain>(Guid.NewGuid());

            await grain.CreateAsync(new CreateFooCommandModel(
                ProjectId: context.ProjectId,
                Name: name,
                DisplayName: name,
                Description: string.Empty,
                Tags: Array.Empty<string>(),
                FooData: data,
                FooMeta: meta));
        }
        else
        {
            var grain = _factory.GetGrain<IFooGrain>(details.GrainId);

            await grain.UpdateAsync(new UpdateFooCommandModel(
                DisplayName: name,
                Description: string.Empty,
                Tags: Array.Empty<string>(),
                FooData: data,
                FooMeta: meta));
        }

        return CommandResult.Success("ok");
    }
}
