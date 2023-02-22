using Orleans;
using System.CommandLine;
using Xioru.Grain;
using Xioru.Grain.Contracts;
using Xioru.Grain.Contracts.ClusterRegistry;
using Xioru.Grain.Contracts.GrainReadModel;
using Xioru.Grain.Contracts.Project;
using Xioru.Grain.Contracts.ProjectRegistry;
using Xioru.Messaging.Contracts.Channel;
using Xioru.Messaging.Contracts.Command;
using Xioru.Messaging.Contracts.Messenger;
using Xioru.Messaging.Messenger;

namespace Xioru.Messaging.MessengerCommand;

public class StartCommand : AbstractMessengerCommand
{
    private readonly Argument<string> _nameArgument =
        new Argument<string>("name", "new unique project name");

    private readonly Argument<string> _codeArgument =
        new Argument<string>(
            name: "code",
            description: "invite code",
            getDefaultValue: () => string.Empty);

    public StartCommand(IGrainFactory factory) : base(factory)
    {
    }

    protected override Command Command => new Command(
        "start", "start new project")
    {
        _nameArgument,
        _codeArgument
    };

    protected override async Task<CommandResult> ExecuteInternal(MessengerCommandContext context)
    {
        var code = GetArgumentValue(_codeArgument);

        // 1. check invite code if not supervisor
        if (!context.IsSupervisor)
        {
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
        var projectName = GetArgumentValue(_nameArgument);
        if (projectName == "you_project_name" || projectName.Contains(' '))
        {
            return CommandResult.LogicError("Project name cannot contain spaces and be named 'you_project_name'");
        }

        // 3. check project name already used
        var readModel = _factory.GetGrain<IClusterRegistryGrain>(GrainConstants.ClusterStreamId);
        var projectDescription = await readModel.GetProjectIdByNameOrDefaultAsync(projectName);

        if (projectDescription != default)
        {
            return CommandResult.LogicError("Project name already used");
        }

        // 4. create project
        var projectId = Guid.NewGuid();
        var project = _factory.GetGrain<IProjectGrain>(projectId);
        await project.Create(new CreateProjectCommand(
            Name: projectName,
            DisplayName: projectName,
            Description: string.Empty));

        var success = await _factory.CheckGrainExistsInProjectAsync(projectId, projectId);

        if (!success)
        {
            throw new Exception("Project not created");
        }

        // 5. create channel
        var channelId = Guid.NewGuid();
        var channel = _factory.GetGrain<IChannelGrain>(channelId);
        await channel.CreateAsync(new CreateChannelCommandModel(
            ProjectId: projectId,
            Name: $"{context.MessengerType}-{context.ChatId}",
            DisplayName: $"{context.MessengerType}-{context.ChatId}",
            Description: String.Empty,
            Tags: Array.Empty<string>(),
            //
            MessengerType: context.MessengerType,
            ChatId: context.ChatId));

        var channelCreationSuccess = await _factory.CheckGrainExistsInProjectAsync(projectId, channelId);

        if (!channelCreationSuccess)
        {
            throw new Exception("Channel not created");
        }

        // 6. Set channel current
        await context.Manager.JoinToProject(
            context.ChatId,
            channelId,
            projectName,
            projectId);

        if (!context.IsSupervisor && !string.IsNullOrWhiteSpace(code))
        {
            await context.Manager.DeleteInvite(code);
        }

        return CommandResult.Success($"Welcome to {projectName} project");
    }
}
