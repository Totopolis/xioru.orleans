using Orleans;
using Xioru.Messaging.Contracts.Command;
using Xioru.Messaging.Contracts.Formatting;

namespace Xioru.Messaging.Contracts.Channel
{
    public interface IChannelGrain : IGrainWithGuidKey
    {
        Task Create(CreateChannelCommand createCommand);

        Task Update(UpdateChannelCommand updateCommand);

        Task Delete();

        Task<ChannelProjection> GetProjection();

        Task SendMessage(FormattedString textMessage);

        Task<CommandResult> ExecuteCommand(string command, bool isSupervisor = false);
    }
}
