using Orleans;
using Xioru.Grain.Contracts;
using Xioru.Grain.Contracts.ProjectReadModel;
using Xioru.Messaging.Contracts.Channel;
using Xioru.Messaging.Contracts.Command;
using Xioru.Messaging.Contracts.Messenger;
using Xioru.Messaging.Messenger;

namespace Xioru.Messaging.MessengerCommand
{
    public class ScdCommand : BaseMessengerCommand
    {
        public const string UsageConst = "/s-cd projectname";

        public ScdCommand(IGrainFactory factory) : base(
            factory: factory,
            commandName: "s-cd",
            subCommandName: string.Empty,
            minArgumentsCount: 1,
            maxArgumentsCount: 1,
            usage: UsageConst)
        {
        }

        protected override async Task<CommandResult> ExecuteInternal(MessengerCommandContext context)
        {
            if (!context.IsSupervisor)
            {
                return CommandResult.LogicError("Command not found");
            }

            var readModel = _factory.GetGrain<IProjectReadModelGrain>(
                GrainConstants.ClusterStreamId);

            var projectName = context.Arguments[0];
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
                await channel.Create(new CreateChannelCommand(
                    ProjectId: project.Id,
                    Name: $"{context.MessengerType}-{context.ChatId}",
                    DisplayName: $"{context.MessengerType}-{context.ChatId}",
                    Description: string.Empty,
                    Tags: new List<string>().ToArray(),
                    //
                    MessengerType: context.MessengerType,
                    MessengerId: context.MessengerId,
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
