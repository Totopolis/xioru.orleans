using Orleans;
using Xioru.Messaging.Contracts.Command;
using Xioru.Messaging.Contracts.Messenger;
using Xioru.Messaging.Messenger;

namespace Xioru.Messaging.MessengerCommand
{
    public class SinviteCommand : BaseMessengerCommand
    {
        public const string UsageConst = "/s-invite";

        public SinviteCommand(IGrainFactory factory) : base(
            factory: factory,
            commandName: "s-invite",
            subCommandName: string.Empty,
            minArgumentsCount: 0,
            maxArgumentsCount: 0,
            usage: UsageConst)
        {
        }

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
