using ConsoleTables;
using Orleans;
using System.CommandLine;
using System.Text;
using Xioru.Messaging.Contracts.Command;
using Xioru.Messaging.Contracts.Messenger;
using Xioru.Messaging.Messenger;

namespace Xioru.Messaging.MessengerCommand;

public class PwdCommand : AbstractMessengerCommand
{
    public PwdCommand(IGrainFactory factory) : base(factory)
    {
    }

    protected override Command Command => new Command(
        "pwd", "display current project");

    protected override Task<CommandResult> ExecuteInternal(MessengerCommandContext context)
    {
        if (!context.Manager.TryGetChannels(context.ChatId, out var channels))
        {
            return Task.FromResult(CommandResult.Success("No accessed projects"));
        }

        var sb = new StringBuilder();
        sb.AppendLine("> List of the accessible projects");
        sb.Append("```");

        var table = new ConsoleTable("Name", "Current");

        foreach (var it in channels)
        {
            table.AddRow(it.ProjectName, it.IsCurrent ? "X" : string.Empty);
        }

        sb.Append($"{table.ToMinimalString()}```");

        return Task.FromResult(CommandResult.Success(sb.ToString()));
    }
}
