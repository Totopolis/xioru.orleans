using Orleans;
using Xioru.Messaging.Contracts.Command;
using Xioru.Messaging.Contracts.Messenger;
using Xioru.Messaging.Messenger;

namespace Xioru.Messaging.MessengerCommand
{
    public class CdCommand : BaseMessengerCommand
    {
        public const string UsageConst = "/cd project-name";

        public CdCommand(IGrainFactory factory) : base(
            factory: factory,
            commandName: "cd",
            subCommandName: String.Empty,
            minArgumentsCount: 1,
            maxArgumentsCount: 1,
            usage: UsageConst)
        {
        }

        protected override Task<CommandResult> ExecuteInternal(MessengerCommandContext context)
        {
            if (!context.Manager.TryGetChannels(context.ChatId, out var channels))
            {
                return Task.FromResult(CommandResult.Success("No accessed projects"));
            }

            var projectName = context.Arguments[0];

            if (!channels.Any(x => x.ProjectName == projectName))
            {
                return Task.FromResult(CommandResult.LogicError("Project name not found"));
            }

            context.Manager.SetCurrentProject(context.ChatId, projectName);

            return Task.FromResult(CommandResult.Success("Current project changed"));
        }
    }
}
