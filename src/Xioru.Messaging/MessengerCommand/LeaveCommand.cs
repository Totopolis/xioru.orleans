using Orleans;
using System.CommandLine;
using Xioru.Messaging.Contracts.Command;
using Xioru.Messaging.Contracts.Messenger;
using Xioru.Messaging.Messenger;

namespace Xioru.Messaging.MessengerCommand
{
    public class LeaveCommand : BaseMessengerCommand
    {
        public LeaveCommand(IGrainFactory factory) : base(factory)
        {
        }

        public override Command Command => new Command(
            "leave", "leave project");

        protected override async Task<CommandResult> ExecuteInternal(MessengerCommandContext context)
        {
            if (!context.Manager.TryGetChannels(context.ChatId, out var channels))
            {
                return CommandResult.LogicError("Selected project not found");
            }

            var selectedProject = channels
                .Where(x => x.IsCurrent)
                .FirstOrDefault();

            if (selectedProject == null)
            {
                return CommandResult.LogicError("Selected project not found");
            }

            await context.Manager.LeaveProject(
                context.ChatId,
                selectedProject.ChannelId,
                selectedProject.ProjectName);

            return CommandResult.Success($"You left the project {selectedProject.ProjectName}");
        }
    }
}
