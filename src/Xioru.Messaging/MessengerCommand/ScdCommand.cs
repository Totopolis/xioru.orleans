using Orleans;
using System.CommandLine;
using Xioru.Grain;
using Xioru.Grain.Contracts;
using Xioru.Grain.Contracts.ClusterRegistry;
using Xioru.Grain.Contracts.GrainReadModel;
using Xioru.Grain.Contracts.ProjectRegistry;
using Xioru.Messaging.Contracts.Channel;
using Xioru.Messaging.Contracts.Command;
using Xioru.Messaging.Contracts.Messenger;
using Xioru.Messaging.Messenger;

namespace Xioru.Messaging.MessengerCommand;

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

        var readModel = _factory.GetGrain<IClusterRegistryGrain>(
            GrainConstants.ClusterStreamId);

        var projectName = GetArgumentValue(_nameArgument);
        var projectId = await readModel.GetProjectIdByNameOrDefaultAsync(projectName);

        if (projectId == default)
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
                ProjectId: projectId.Value,
                Name: $"{context.MessengerType}-{context.ChatId}",
                DisplayName: $"{context.MessengerType}-{context.ChatId}",
                Description: string.Empty,
                Tags: new List<string>().ToArray(),
                //
                MessengerType: context.MessengerType,
                ChatId: context.ChatId));

            var success = await _factory.CheckGrainExistsInProjectAsync(projectId.Value, channelId);

            if (!success)
            {
                throw new Exception("Channel not created");
            }

            await context.Manager.JoinToProject(
                context.ChatId,
                channelId,
                projectName,
                projectId.Value);

            return CommandResult.Success($"Project {projectName} joined");
        }
    }
}
