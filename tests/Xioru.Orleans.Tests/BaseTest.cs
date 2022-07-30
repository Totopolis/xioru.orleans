using System;
using System.Linq;
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

            var project = await _projectReadModel
                .GetProjectByName(_projectName);
            Assert.NotNull(project);

            var channel = await _grainReadModel
                .GetGrainByName(_channelId.ToString());
            Assert.NotNull(channel);
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

            await InternalCreateFoo("Foo");
            var details = await _grainReadModel.GetGrainByName("Foo");
            Assert.NotNull(details);
            Assert.Equal("Foo", details!.GrainName);
        }

        [Fact]
        public async Task FindFooByFilter()
        {
            await PrepareAsync();

            await InternalCreateFoo("Foo");
            await InternalCreateFoo("Bob");

            var foo = (await _grainReadModel.GetGrains("oo")).FirstOrDefault();
            Assert.NotNull(foo);
            Assert.Equal("Foo", foo!.GrainName);
        }

        private async Task<IFooGrain> InternalCreateFoo(string name)
        {
            var foo = _factory.GetGrain<IFooGrain>(Guid.NewGuid());
            await foo.Create(new CreateFooCommand(
                ProjectId: _projectId,
                Name: name,
                DisplayName: name,
                Description: string.Empty,
                Tags: new string[0],
                FooData: $"Hello {name}"));

            return foo;
        }
    }
}
