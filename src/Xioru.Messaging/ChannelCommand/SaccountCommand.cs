using System.CommandLine;
using Xioru.Grain.Contracts.Account;
using Xioru.Messaging.Contracts.Channel;
using Xioru.Messaging.Contracts.Command;

namespace Xioru.Messaging.ChannelCommand;

public partial class SaccountCommand : AbstractChannelCommand
{
    private readonly Argument<string> _idArgument =
        new Argument<string>("id", "email or phone number");

    private readonly Argument<string> _passwordArgument =
        new Argument<string>("password", "account password");

    public SaccountCommand(IGrainFactory factory) : base(factory)
    {
    }

    protected override Command Command => new Command(
        "s-account", "force create account and join to the current project (supervisor)")
    {
        _idArgument,
        _passwordArgument
    };

    protected override async Task<CommandResult> ExecuteInternal(
        ChannelCommandContext context)
    {
        var id = GetArgumentValue(_idArgument);
        var password = GetArgumentValue(_passwordArgument);

        var account = _factory.GetGrain<IAccountGrain>(id);
        await account.ForceCreate(password);
        await account.InviteToProject(context.ProjectId);

        return CommandResult.Success($"Account '{id}' succefully created");

    }
}
