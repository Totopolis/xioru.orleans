using System.Linq;
using System.Threading.Tasks;
using Xioru.Orleans.Tests.Common;
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

            var channel = await _grainReadModel.GetGrainDetailsByName(_channelName);
            Assert.NotNull(channel);
        }

        [Fact]
        public async Task CheckChannel()
        {
            await PrepareAsync();

            var channelProjection = await _channel.GetProjection();
            Assert.NotNull(channelProjection);

            await _channel.SendMessage("Hello");
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
            var details = await _grainReadModel.GetGrainDetailsByName("Foo");
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

        [Fact]
        public async Task FindProjectById()
        {
            await PrepareAsync();

            var project = await _projectReadModel.GetProjectById(_projectId);
            Assert.NotNull(project);
        }
    }
}
