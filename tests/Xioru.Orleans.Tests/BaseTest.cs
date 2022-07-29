using System;
using System.Threading.Tasks;
using Xioru.Orleans.Tests.Common;
using Xioru.Orleans.Tests.Foo;
using Xunit;

namespace Xioru.Orleans.Tests
{
    [Collection(TestsCollection.Name)]
    public class BaseTest : AbstractTest
    {
        public BaseTest(TestsFixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async Task CheckProjectAndChannelCreated()
        {
            await PrepareAsync();

            var project = await _projectReadModel.GetProjectByName(_projectName);
            Assert.NotNull(project);
        }

        [Fact]
        public async Task CheckGrainReadModel()
        {
            await PrepareAsync();

            var details = await _grainReadModel.GetGrains();
            Assert.Equal(2, details.Count);
        }

        [Fact]
        public async Task CreateFooGrain()
        {
            await PrepareAsync();

            var foo = _factory.GetGrain<IFooGrain>(Guid.NewGuid());
            await foo.Create(new CreateFooCommand(
                ProjectId: _projectId,
                Name: "Foo1",
                DisplayName: string.Empty,
                Description: string.Empty,
                Tags: new string[0],
                FooData: "Hello Foo1"));
        }
    }
}
