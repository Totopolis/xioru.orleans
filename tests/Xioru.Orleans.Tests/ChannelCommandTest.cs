using System.Threading.Tasks;
using Xioru.Orleans.Tests.Common;
using Xunit;

namespace Xioru.Orleans.Tests
{
    [Collection(TestsCollection.Name)]
    public class ChannelCommandTest : AbstractTest
    {
        public ChannelCommandTest(TestsFixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async Task CheckListCommand()
        {
            await PrepareAsync();
            var foo = await InternalCreateFoo("Foo1");

            var result = await _channel.ExecuteCommand("/list");
            var text = result.Message.ToString();

            Assert.True(result.IsSuccess);
            Assert.False(string.IsNullOrEmpty(text));

            Assert.Contains(_projectName, text);
            Assert.Contains("Project", text);

            Assert.Contains(_channelName, text);
            Assert.Contains("Channel", text);

            Assert.Contains("Foo1", text);
            Assert.Contains("Foo", text);
        }

        [Fact]
        public async Task CheckDetailsCommand()
        {
            await PrepareAsync();
            var foo = await InternalCreateFoo("Foo1");
            
            var result = await _channel.ExecuteCommand("/details Foo1");
            var text = result.Message.ToString();

            Assert.True(result.IsSuccess);
            Assert.False(string.IsNullOrEmpty(text));

            Assert.Contains("Foo", text);
        }
    }
}
