using System.Linq;
using Xioru.Messaging.Contracts.Formatting;
using Xioru.Orleans.Tests.Common;
using Xunit;

namespace Xioru.Orleans.Tests;

[Collection(TestsCollection.Name)]
public class FormattedStringTest
{
    [Theory]
    [InlineData("simpleString")]
    [InlineData("Spaced String")]
    [InlineData("")]
    [InlineData("\nstrings\nwith\tsymbols\n")]
    public void ToString_SimpleString_CorrectOutput(string input)
    {
        var formattedString = new FormattedString(input);

        var output = formattedString.ToString(x => x, x => x);

        Assert.Equal(input, output);
    }

    [Fact]
    public void ToString_WithFormatterFuncs_CorrectOutput()
    {
        var expectedOutput = "text *bold* _italic_ [box]";
        var formattedString = new FormattedString("text ")
            .Append("bold", StringFormatting.Bold)
            .Append(" ")
            .Append("italic", StringFormatting.Italic)
            .Append(" ")
            .Append("box", StringFormatting.BoxedLine);

        var output = formattedString.ToString(x => $"*{x}*", x => $"[{x}]", x => $"_{x}_");

        Assert.Equal(expectedOutput, output);
    }

    [Theory]
    [InlineData("simple", "simple" )]
    [InlineData("(str)", "_(str_)" )]
    [InlineData("\ttab", "____tab" )]
    public void ToString_Replaces_CorrectOutput(string input, string expectedOutput)
    {
        var formattedString = new FormattedString(input);

        var output = formattedString.ToString(x => x, x => x, replaces: new() 
        { 
            { ":", "_:" },
            { "(", "_(" },
            { ")", "_)" },
            { "\t", "____" },
        });

        Assert.Equal(expectedOutput, output);
    }

    [Theory]
    [InlineData(5000)]
    [InlineData(4001)]
    [InlineData(20000)]
    public void ToString_Limit_CorrectOutput(int simbolCount)
    {
        var limit = 4000;
        var formattedString = new FormattedString(new string('x', simbolCount));

        var output = formattedString.ToString(x => x, x => x, limit: limit);

        Assert.True(output.Length <= limit);
    }

    [Fact]
    public void ToString_LimitWithReplaces_CorrectOutput()
    {
        var limit = 4000;
        var lines = Enumerable.Range(0, 3000)
            .Select(x => $"(i{x})");
        var formattedString = new FormattedString("Header:")
            .Append(string.Join('\t', lines));


        var output = formattedString.ToString(x => x, x => x, replaces: new()
        {
            { "(", "_(" },
            { ")", "_)" },
            { "\t", "____" },
        }, 
        limit: limit);

        Assert.True(output.Length <= limit);
    }
}
