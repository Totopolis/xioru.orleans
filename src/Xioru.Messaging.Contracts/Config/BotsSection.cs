namespace Xioru.Messaging.Contracts.Config
{
    public class BotsConfigSection
    {
        public const string SectionName = "Bots";

        public MessengerSection? Discord { get; set; }

        public MessengerSection? Telegram { get; set; }
    }
}
