namespace Xioru.Messaging.Contracts
{
    public static class MessagingConstants
    {
        /// <summary>
        /// from messenger to channel (streamId=channelId)
        /// </summary>
        public const string ChannelIncomingStreamNamespace = "ChannelIncomingStream";

        /// <summary>
        /// from channel to messenger (streamId=messengerId)
        /// </summary>
        public const string ChannelOutcomingStreamNamespace = "ChannelOutcomingStream";
    }
}
