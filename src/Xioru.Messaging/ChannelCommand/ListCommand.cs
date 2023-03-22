using ConsoleTables;
using System.CommandLine;
using Xioru.Grain.Contracts.ProjectRegistry;
using Xioru.Messaging.Contracts.Channel;
using Xioru.Messaging.Contracts.Command;
using Xioru.Messaging.Contracts.Formatting;

namespace Xioru.Messaging.ChannelCommand;

public class ListCommand : AbstractChannelCommand
{
    private readonly Argument<string> _filterArgument = new Argument<string>(
            name: "filter",
            description: "substring to search in",
            getDefaultValue: () => string.Empty);

    public ListCommand(IGrainFactory factory) : base(factory)
    {
    }

    protected override Command Command => new Command(
        "list", "display all objects")
    {
        _filterArgument
    };

    protected override async Task<CommandResult> ExecuteInternal(
        ChannelCommandContext context)
    {
        var filterValue = GetArgumentValue(_filterArgument);

        if (!string.IsNullOrWhiteSpace(filterValue) && filterValue.Length < 3)
        {
            throw new CommandLogicErrorException(
                "Filter must be at least 3 characters long");
        }

        // TODO: use filter
        var grainDetails = await _factory.GetGrain<IProjectRegistryGrain>(context.ProjectId)
            .GetGrains();

        if (!grainDetails!.Any())
        {
            return CommandResult.Success("No objects found");
        }

        var fString = new FormattedString("List of the platform objects", StringFormatting.BoxedLine);

        var table = new ConsoleTable("Name", "Type");

        foreach (var grain in grainDetails)
        {
            var shortenedGrainType = grain.GrainType.LastIndexOf('.') > 0
                ? grain.GrainType.Substring(grain.GrainType.LastIndexOf('.'))
                : grain.GrainType;
            
            var trimmedTypeName = shortenedGrainType.EndsWith("Grain")
                ? shortenedGrainType.Substring(0, shortenedGrainType.Length - "Grain".Length)
                : shortenedGrainType;
            table.AddRow(grain.GrainName, trimmedTypeName);
        }

        fString.Append($"{table.ToMinimalString()}", StringFormatting.Code);

        return CommandResult.Success(fString);
    }
}
