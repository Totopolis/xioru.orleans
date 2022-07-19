using Xioru.Messaging.Contracts.Messenger;

namespace Xioru.Messaging.Contracts.Config
{
    public class BotsConfigSection
    {
        public const string SectionName = "Bots";

        public Dictionary<MessengerType, MessengerSection> Configs { get; set; } = new Dictionary<MessengerType, MessengerSection>();
    }
}
