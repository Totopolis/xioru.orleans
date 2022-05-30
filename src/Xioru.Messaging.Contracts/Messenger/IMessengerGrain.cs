using Orleans;
using Xioru.Messaging.Contracts.Config;

namespace Xioru.Messaging.Contracts.Messenger
{
    public interface IMessengerGrain : IGrainWithGuidKey
    {
        Task StartAsync(MessengerSection config);
    }
}
