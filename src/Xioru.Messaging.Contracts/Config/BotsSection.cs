using Xioru.Messaging.Contracts.Messenger;

namespace Xioru.Messaging.Contracts.Config
{
    public class BotsSection
    {
        public const string SectionName = "Bots";

        public Dictionary<MessengerType, MessengerSection> Configs { get; set; } = new();
    }
}
