using Xioru.Messaging.Contracts.Messenger;

namespace Xioru.Messaging.Contracts;

public static class MessagingConstants
{
    /// <summary>
    /// from messenger to channel (streamId=channelId)
    /// </summary>
    public const string ChannelIncomingStreamNamespace = "ChannelIncomingStream";

    /// <summary>
    /// from channel to messenger (streamId=Guid.Empty)
    /// </summary>
    public static string GetChannelOutcomingStreamNamespace(MessengerType type) => $"ChannelOutcomingStream_{Enum.GetName(type)}";
}
