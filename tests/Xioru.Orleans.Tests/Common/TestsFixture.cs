using Orleans.TestingHost;
using System;

namespace Xioru.Orleans.Tests.Common;

public class TestsFixture : IDisposable
{
    public TestsFixture()
    {
        Cluster = new TestClusterBuilder(initialSilosCount: 1)
            .AddSiloBuilderConfigurator<ClusterConfigurator>()
            .Build();

        Cluster.Deploy();
    }

    public void Dispose()
    {
        Cluster.StopAllSilos();
    }

    public TestCluster Cluster { get; init; }
}
