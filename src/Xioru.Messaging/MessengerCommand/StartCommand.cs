using Orleans;
using Xioru.Grain.Contracts;
using Xioru.Grain.Contracts.Project;
using Xioru.Grain.Contracts.ProjectReadModel;
using Xioru.Messaging.Contracts.Channel;
using Xioru.Messaging.Contracts.Command;
using Xioru.Messaging.Contracts.Messenger;
using Xioru.Messaging.Messenger;

namespace Xioru.Messaging.MessengerCommand
{
    public class StartCommand : BaseMessengerCommand
    {
        public const string UsageConst = "/start you_project_name invite-code";

        public StartCommand(IGrainFactory factory) : base(
            factory: factory,
            commandName: "start",
            subCommandName: String.Empty,
            minArgumentsCount: 1, // wo code for supervisers
            maxArgumentsCount: 2,
            usage: UsageConst)
        {
        }

        protected override async Task<CommandResult> ExecuteInternal(MessengerCommandContext context)
        {
            // 1. check invite code if not supervisor
            if (!context.IsSupervisor)
            {
                var code = context.Arguments[1];
                if (!context.Manager.CheckInvite(code, out var findedProject))
                {
                    return CommandResult.LogicError("Invite code not found");
                }

                if (findedProject != default)
                {
                    return CommandResult.InternalError("Bad invite code");
                }
            }

            // 2. check new project name
            var projectName = context.Arguments[0];
            if (projectName == "you_project_name" || projectName.Contains(' '))
            {
                return CommandResult.LogicError("Project name cannot contain spaces and be named 'you_project_name'");
            }

            // 3. check project name already used
            var readModel = _factory.GetGrain<IProjectReadModelGrain>(GrainConstants.ClusterStreamId);
            var projectDescription = await readModel.GetProjectByName(projectName);

            if (projectDescription != default)
            {
                return CommandResult.LogicError("Project name already used");
            }

            // 4. create project
            var projectId = Guid.NewGuid();
            var project = _factory.GetGrain<IProjectGrain>(projectId);
            await project.Create(new CreateProjectCommand
            {
                Name = projectName,
                DisplayName = projectName,
                Description = String.Empty
            });

            // 5. create channel
            var channelId = Guid.NewGuid();
            var channel = _factory.GetGrain<IChannelGrain>(channelId);
            await channel.Create(new CreateChannelCommand(
                ProjectId: projectId,
                Name: $"{context.MessengerType}-{context.ChatId}",
                DisplayName: $"{context.MessengerType}-{context.ChatId}",
                Description: String.Empty,
                Tags: new List<string>().ToArray(),
                //
                MessengerType: context.MessengerType,
                ChatId: context.ChatId));

            // 6. Set channel current
            await context.Manager.JoinToProject(
                context.ChatId,
                channelId,
                projectName,
                projectId);

            if (!context.IsSupervisor &&
                context.ArgsCount == 2 &&
                !string.IsNullOrWhiteSpace(context.Arguments[1]))
            {
                await context.Manager.DeleteInvite(context.Arguments[1]);
            }

            return CommandResult.Success($"Welcome to {projectName} project");
        }
    }
}
