using Microsoft.Extensions.Configuration;
using Orleans.TestingHost;
using System;

namespace Xioru.Orleans.Tests.Common
{
    public class TestsFixture : IDisposable
    {
        public TestsFixture()
        {
            Cluster = new TestClusterBuilder()
                .AddSiloBuilderConfigurator<ClusterConfigurator>()
                .ConfigureHostConfiguration(configurationBuilder =>
                {
                    configurationBuilder.AddEnvironmentVariables();
                })
                .Build();

            Cluster.Deploy();
        }

        public void Dispose()
        {
            Cluster.StopAllSilos();
        }

        public TestCluster Cluster { get; init; }
    }
}
