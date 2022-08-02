using Orleans;
using Xioru.Grain.Contracts.Project;
using Xioru.Messaging.Contracts.Channel;
using Xioru.Messaging.Contracts.Command;
using Xioru.Messaging.Contracts.Messenger;
using Xioru.Messaging.Messenger;

namespace Xioru.Messaging.MessengerCommand
{
    public class JoinCommand : BaseMessengerCommand
    {
        public const string UsageConst = "/join invite-code";

        public JoinCommand(IGrainFactory factory) : base(
            factory: factory,
            commandName: "join",
            subCommandName: String.Empty,
            minArgumentsCount: 1,
            maxArgumentsCount: 1,
            usage: UsageConst)
        {
        }

        protected override async Task<CommandResult> ExecuteInternal(MessengerCommandContext context)
        {
            var code = context.Arguments[0];
            if (string.IsNullOrWhiteSpace(code))
            {
                return CommandResult.LogicError("Bad code argument");
            }

            if (!context.Manager.CheckInvite(code, out var projectId))
            {
                return CommandResult.LogicError("Invite code not found");
            }

            if (projectId == default!)
            {
                return CommandResult.InternalError("Project not found");
            }

            // TODO: check chatId in project already

            // check project exists
            var project = _factory.GetGrain<IProjectGrain>(projectId);
            var descr = await project.GetProjection();

            // create channel
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

            //
            await context.Manager.JoinToProject(
                context.ChatId,
                channelId,
                descr.Name,
                projectId);

            await context.Manager.DeleteInvite(code);

            return CommandResult.Success($"Welcome to {descr.Name} project");
        }
    }
}
