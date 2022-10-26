using System.Text;

namespace Xioru.Messaging.Contracts.Formatting;

public class FormattedString
{
    private const string _footerComment = "\n\nСообщение усечено. Уточните запрос";
    public FormattedString(string str, StringFormatting formatting = StringFormatting.None)
    {
        _formattedElements = new List<(string, StringFormatting)> { (str, formatting) };
    }

    public FormattedString()
    {
        _formattedElements = new List<(string, StringFormatting)>();
    }

    private List<(string Text, StringFormatting Formatting)> _formattedElements;

    public FormattedString Append(string text, StringFormatting formatting = StringFormatting.None)
    {
        _formattedElements.Add((text, formatting));
        return this;
    }

    public FormattedString Append(FormattedString formattedString)
    {
        _formattedElements.AddRange(formattedString._formattedElements);
        return this;
    }

    public override string ToString()
    {
        return string.Join(string.Empty, _formattedElements);
    }

    public string ToString(
        Func<string, string> boldFormatter,
        Func<string, string> boxedLineFormatter,
        Func<string, string>? italicFormatter = null,
        Func<string, string>? underlineFormatter = null,
        Func<string, string>? codeFormatter = null,
        Dictionary<string, string>? replaces = null,
        int limit = int.MaxValue)
    {
        var limitWithComment = limit - _footerComment.Length;
        var builder = new StringBuilder();
        foreach(var elm in _formattedElements)
        {
            var strApplied = 
                ApplyFormatRules(Escape(elm.Text, replaces), elm.Formatting);

            if (builder.Length + strApplied.Length > limit)
            {
                var formatedFooter = ApplyFormatRules(Escape(_footerComment, replaces), StringFormatting.Bold);
                var serviceAdditionLength = strApplied.Length - elm.Text.Length;
                var rawLength = limitWithComment - builder.Length - serviceAdditionLength;
                if (rawLength > 0)
                {
                    var cuttedText = elm.Text.Substring(0, rawLength);
                    strApplied = 
                        ApplyFormatRules(Escape(cuttedText, replaces), elm.Formatting);
                    strApplied += formatedFooter;
                }
                builder.Append(strApplied);
                break;
            }

            builder.Append(strApplied);
        };

        return builder.ToString();

        string ApplyFormatRules(string text, StringFormatting formatting)
        {
            if (formatting.HasFlag(StringFormatting.Bold)) text = boldFormatter(text);
            if (formatting.HasFlag(StringFormatting.Italic) && italicFormatter != null)
                text = italicFormatter(text);
            if (formatting.HasFlag(StringFormatting.Underline) && underlineFormatter != null)
                text = underlineFormatter(text);
            if (formatting.HasFlag(StringFormatting.Code) && codeFormatter != null)
                text = codeFormatter(text);
            if (formatting.HasFlag(StringFormatting.BoxedLine))
                text = boxedLineFormatter(text);
            return text;
        }
    }

    private string Escape(string rawText, Dictionary<string, string>? replaces)
    {
        var ret = rawText;
        if (replaces != null)
        {
            foreach (var c in replaces.Keys)
            {
                ret = ret.Replace(c, replaces[c]);
            }
        }
        return ret;
    }

    public static implicit operator string(FormattedString self)
    {
        return self.ToString();
    }

    public static implicit operator FormattedString(string self)
    {
        return new FormattedString(self);
    }
}

[Flags]
public enum StringFormatting
{
    None = 0,
    Bold = 1,
    Italic = 2,
    Underline = 4,
    Code = 8,
    BoxedLine = 16
}
