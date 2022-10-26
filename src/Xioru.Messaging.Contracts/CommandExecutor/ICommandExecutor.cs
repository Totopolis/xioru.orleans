using Orleans;
using Xioru.Messaging.Contracts.Command;

namespace Xioru.Messaging.Contracts.CommandExecutor;

public interface ICommandExecutor : IGrainWithGuidKey
{
    Task<CommandResult> Execute(
        Guid projectId,
        Guid channelId,
        bool isSupervisor,
        string commandText);
}
