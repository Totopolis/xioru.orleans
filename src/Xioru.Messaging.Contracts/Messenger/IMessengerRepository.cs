namespace Xioru.Messaging.Contracts.Messenger
{
    public interface IMessengerRepository // TODO: consider making two interfaces out ot this one (ISP)
    {
        Task StartAsync(MessengerType type);

        bool TryGetCurrentChannel(string chatId, out AccessRecord channel);

        bool TryGetChat(Guid channelId, out string chatId);

        bool TryGetChannels(string chatId, out AccessRecord[] channels);

        Task SetCurrentProject(string chatId, string projectName);

        Task JoinToProject(
            string chatId,
            Guid channelId,
            string projectName,
            Guid projectId);

        Task LeaveProject(
            string chatId,
            Guid channelId,
            string projectName);

        Task CreateInvite(string code, Guid projectId);

        bool CheckInvite(string code, out Guid projectId);

        Task DeleteInvite(string code);

        Task OnProjectDeleted(Guid projectId);

        Task OnChannelDeleted(Guid channelId);
    }
}
