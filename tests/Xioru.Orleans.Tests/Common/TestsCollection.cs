using Xunit;

namespace Xioru.Orleans.Tests.Common
{
    [CollectionDefinition(TestsCollection.Name)]
    public class TestsCollection : ICollectionFixture<TestsFixture>
    {
        public const string Name = "TestsCollection";
    }
}
