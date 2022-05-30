using Orleans;
using Xioru.Messaging.Contracts.Command;
using Xioru.Messaging.Contracts.Messenger;
using Xioru.Messaging.Messenger;

namespace Xioru.Messaging.MessengerCommand
{
    public class InviteCommand : BaseMessengerCommand
    {
        public const string UsageConst = "/invite";

        public InviteCommand(IGrainFactory factory) : base(
            factory: factory,
            commandName: "invite",
            subCommandName: String.Empty,
            minArgumentsCount: 0,
            maxArgumentsCount: 0,
            usage: UsageConst)
        {
        }

        protected override Task<CommandResult> ExecuteInternal(MessengerCommandContext context)
        {
            if (!context.Manager.TryGetChannels(context.ChatId, out var channels))
            {
                return Task.FromResult(CommandResult.LogicError("No accessed projects"));
            }

            var currentChannel = channels.FirstOrDefault(x => x.IsCurrent);
            if (currentChannel == null)
            {
                return Task.FromResult(CommandResult.LogicError("No current project found"));
            }

            var code = Guid.NewGuid().ToString("N");
            context.Manager.CreateInvite(code, currentChannel.ProjectId);

            var result = $"Invite to project '{currentChannel.ProjectName}' created" + Environment.NewLine;
            result += "Provide command to user:" + Environment.NewLine;
            result += $"/join {code}";

            return Task.FromResult(CommandResult.Success(result));
        }
    }
}
