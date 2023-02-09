using ConsoleTables;
using Orleans;
using System.CommandLine;
using System.Text;
using Xioru.Grain.Contracts;
using Xioru.Grain.Contracts.ProjectReadModel;
using Xioru.Messaging.Contracts.Command;
using Xioru.Messaging.Contracts.Formatting;
using Xioru.Messaging.Contracts.Messenger;
using Xioru.Messaging.Messenger;

namespace Xioru.Messaging.MessengerCommand;

public class SpwdCommand : AbstractMessengerCommand
{
    private readonly Argument<string> _filterArgument = new Argument<string>(
        name: "filter",
        description: "substring to search in project names",
        getDefaultValue: () => string.Empty);

    public SpwdCommand(IGrainFactory factory) : base(factory)
    {
    }

    protected override Command Command => new Command(
        "s-pwd", "display all projects in the system (supervisor)")
    {
        _filterArgument
    };

    protected override async Task<CommandResult> ExecuteInternal(MessengerCommandContext context)
    {
        if (!context.IsSupervisor)
        {
            return CommandResult.LogicError("Command not found");
        }

        context.Manager.TryGetChannels(context.ChatId, out var channels);

        var filter = GetArgumentValue(_filterArgument);

        var readModel = _factory.GetGrain<IProjectReadModelGrain>(
            GrainConstants.ClusterStreamId);

        var projects = await readModel.GetProjectsByFilter(filter);

        var fString = new FormattedString("List of all user projects", StringFormatting.BoxedLine);

        var table = new ConsoleTable("Id", "Name", "Current");

        foreach (var it in projects)
        {
            table.AddRow(
                it.Id,
                it.Name,
                channels.FirstOrDefault(
                    x => x.ProjectName == it.Name && x.IsCurrent) == null ?
                    string.Empty : "X");
        }

        fString.Append($"{table.ToMinimalString()}", StringFormatting.Code);

        return CommandResult.Success(fString);
    }
}
