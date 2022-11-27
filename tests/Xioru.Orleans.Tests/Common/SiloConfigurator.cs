using Orleans.Hosting;
using Orleans.TestingHost;
using Xioru.Grain.Contracts;
using Xioru.Messaging;

namespace Xioru.Orleans.Tests.Common;

public class SiloConfigurator : ISiloConfigurator
{
    public void Configure(ISiloBuilder builder)
    {
        builder.AddMemoryStreams(
            GrainConstants.StreamProviderName,
            configure =>
            {
            });

        builder
            .AddMemoryGrainStorage(GrainConstants.StateStorageName)
            .AddMemoryGrainStorage("PubSubStore")
            .UseInMemoryReminderService();

        builder.AddStartupTask<MessagingStartupTask>();
    }
}
