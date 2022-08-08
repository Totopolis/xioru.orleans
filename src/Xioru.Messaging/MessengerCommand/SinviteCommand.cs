using Orleans;
using System.CommandLine;
using Xioru.Messaging.Contracts.Command;
using Xioru.Messaging.Contracts.Messenger;
using Xioru.Messaging.Messenger;

namespace Xioru.Messaging.MessengerCommand
{
    public class SinviteCommand : BaseMessengerCommand
    {
        public SinviteCommand(IGrainFactory factory) : base(factory)
        {
        }

        public override Command Command => new Command(
           "s-invite", "create invite code to create new project (supervisor)");

        protected override Task<CommandResult> ExecuteInternal(MessengerCommandContext context)
        {
            if (!context.IsSupervisor)
            {
                return Task.FromResult(CommandResult.LogicError("Command not found"));
            }

            var code = Guid.NewGuid().ToString("N");
            context.Manager.CreateInvite(code, Guid.Empty);

            var result = $"Invite to create new project created" + Environment.NewLine;
            result += "Provide command to user:" + Environment.NewLine;
            result += $"/start you_project_name {code}";

            return Task.FromResult(CommandResult.Success(result));
        }
    }
}
