using Xioru.Messaging.Contracts.Messenger;

namespace Xioru.Messaging.Contracts.Config
{
    public class MessengerSection
    {
        public bool Enable { get; set; } = false;

        public string Token { get; set; } = default!;

        public string[] Supervisors { get; set; } = default!;
    }
}
