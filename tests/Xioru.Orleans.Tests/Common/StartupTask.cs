using Orleans;
using Orleans.Runtime;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xioru.Orleans.Tests.VirtualMessenger;

namespace Xioru.Orleans.Tests.Common
{
    internal class StartupTask : IStartupTask
    {
        private readonly IGrainFactory _factory;

        public StartupTask(IGrainFactory factory)
        {
            _factory = factory;
        }

        public async Task Execute(CancellationToken cancellationToken)
        {
            var virtualMessenger = _factory.GetGrain<IVirtualMessengerGrain>(
                Guid.Empty);

            await virtualMessenger.StartAsync();
        }
    }

}
