using Orleans;
using System.CommandLine;
using Xioru.Grain;
using Xioru.Messaging.Contracts.Command;
using Xioru.Messaging.Contracts.Messenger;
using Xioru.Messaging.Messenger;

namespace Xioru.Messaging.MessengerCommand;

public class SmailCommand : AbstractMessengerCommand
{
    private readonly Argument<string> _emailArgument =
        new Argument<string>("email", "email to send");

    private readonly Argument<string> _subjectArgument =
        new Argument<string>("subject", "email subject");

    private readonly Argument<string> _bodyArgument =
        new Argument<string>("body", "text body of the email");

    public SmailCommand(
        IGrainFactory factory,
        IServiceProvider services) : base(factory)
    {
    }

    protected override Command Command => new Command(
        "s-mail", "send email")
    {
        _emailArgument,
        _subjectArgument,
        _bodyArgument
    };

    protected override async Task<CommandResult> ExecuteInternal(MessengerCommandContext context)
    {
        var email = GetArgumentValue(_emailArgument);
        var subject = GetArgumentValue(_subjectArgument);
        var body = GetArgumentValue(_bodyArgument);

        await _factory.SendEmail(email, body, subject);

        return CommandResult.Success("Email sended");
    }
}
