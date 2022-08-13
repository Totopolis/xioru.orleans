using Orleans.Runtime;
using Xioru.Messaging.Contracts.Messenger;

namespace Xioru.Messaging
{
    // TODO: non correct - will be started on each silo in the cluster, need fix
    public class MessagingStartupTask : IStartupTask
    {
        private readonly IEnumerable<IMessengerGrain> _messengers;

        public MessagingStartupTask(IEnumerable<IMessengerGrain> messengers)
        {
            _messengers = messengers;
        }

        public async Task Execute(CancellationToken cancellationToken)
        {
            await Task.WhenAll(_messengers.Select(async m => await m.StartAsync()));
        }
    }
}
