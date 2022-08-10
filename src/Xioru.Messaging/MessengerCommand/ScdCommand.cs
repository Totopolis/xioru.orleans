using Orleans;
using System.CommandLine;
using Xioru.Grain.Contracts;
using Xioru.Grain.Contracts.ProjectReadModel;
using Xioru.Messaging.Contracts.Channel;
using Xioru.Messaging.Contracts.Command;
using Xioru.Messaging.Contracts.Messenger;
using Xioru.Messaging.Messenger;

namespace Xioru.Messaging.MessengerCommand
{
    public class ScdCommand : AbstractMessengerCommand
    {
        private readonly Argument<string> _nameArgument =
            new Argument<string>("name", "unique project name");

        public ScdCommand(IGrainFactory factory) : base(factory)
        {
        }

        protected override Command Command => new Command(
            "s-cd", "change current project (supervisor)")
        {
            _nameArgument
        };

        protected override async Task<CommandResult> ExecuteInternal(MessengerCommandContext context)
        {
            if (!context.IsSupervisor)
            {
                return CommandResult.LogicError("Command not found");
            }

            var readModel = _factory.GetGrain<IProjectReadModelGrain>(
                GrainConstants.ClusterStreamId);

            var projectName = GetArgumentValue(_nameArgument);
            var project = await readModel.GetProjectByName(projectName);

            if (project == null)
            {
                return CommandResult.LogicError("Project name not found");
            }

            context.Manager.TryGetChannels(context.ChatId, out var channels);

            var currentChannel = channels
                .Where(x => x.ProjectName == projectName)
                .FirstOrDefault();

            if (currentChannel != null)
            {
                await context.Manager.SetCurrentProject(context.ChatId, projectName);
                return CommandResult.Success($"Project {projectName} selected");
            }
            else
            {
                var channelId = Guid.NewGuid();
                var channel = _factory.GetGrain<IChannelGrain>(channelId);
                await channel.CreateAsync(new CreateChannelCommandModel(
                    ProjectId: project.Id,
                    Name: $"{context.MessengerType}-{context.ChatId}",
                    DisplayName: $"{context.MessengerType}-{context.ChatId}",
                    Description: string.Empty,
                    Tags: new List<string>().ToArray(),
                    //
                    MessengerType: context.MessengerType,
                    ChatId: context.ChatId));

                //
                await context.Manager.JoinToProject(
                    context.ChatId,
                    channelId,
                    project.Name,
                    project.Id);

                return CommandResult.Success($"Project {projectName} joined");
            }
        }
    }
}
