using Orleans;
using System.CommandLine;
using Xioru.Messaging.Contracts.Command;
using Xioru.Messaging.Contracts.Messenger;
using Xioru.Messaging.Messenger;

namespace Xioru.Messaging.MessengerCommand
{
    public class CdCommand : BaseMessengerCommand
    {
        private readonly Argument<string> _nameArgument =
            new Argument<string>("name", "unique project name");

        public CdCommand(IGrainFactory factory) : base(factory)
        {
        }

        public override Command Command => new Command(
            "cd", "navigate to project")
        {
            _nameArgument
        };

        protected override Task<CommandResult> ExecuteInternal(MessengerCommandContext context)
        {
            if (!context.Manager.TryGetChannels(context.ChatId, out var channels))
            {
                return Task.FromResult(CommandResult.Success("No accessed projects"));
            }

            var projectName = context.GetArgumentValue(_nameArgument);

            if (!channels.Any(x => x.ProjectName == projectName))
            {
                return Task.FromResult(CommandResult.LogicError("Project name not found"));
            }

            context.Manager.SetCurrentProject(context.ChatId, projectName);

            return Task.FromResult(CommandResult.Success("Current project changed"));
        }
    }
}
