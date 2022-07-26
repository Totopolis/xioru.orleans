using System.Text;

namespace Xioru.Messaging.Contracts.Formatting
{
    public class FormattedString
    {
        private const string _limitComment = "\n\nСообщение усечено. Уточните запрос";
        public FormattedString(string str, Formatting formatting = Formatting.None)
        {
            _formattedElements = new List<(string, Formatting)> { (str, formatting) };
        }

        public FormattedString()
        {
            _formattedElements = new List<(string, Formatting)>();
        }

        private List<(string Text, Formatting Formatting)> _formattedElements;

        public FormattedString Append(string text, Formatting formatting = Formatting.None)
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
            Func<string, string>? italicFormatter = null,
            Func<string, string>? underlineFormatter = null,
            Dictionary<string, string>? replaces = null,
            int limit = int.MaxValue)
        {
            var limitWithComment = limit - _limitComment.Length;
            var builder = new StringBuilder();
            foreach(var elm in _formattedElements)
            {
                var strApplied = 
                    ApplyFormatRules(Escape(elm.Text, replaces), elm.Formatting);

                if (builder.Length + strApplied.Length > limitWithComment)
                {
                    var serviceAdditionLength = strApplied.Length - elm.Text.Length;
                    var rawLength = limitWithComment - builder.Length - serviceAdditionLength;
                    if (rawLength > 0)
                    {
                        var cuttedText = elm.Text.Substring(0, rawLength);
                        strApplied = 
                            ApplyFormatRules(Escape(cuttedText, replaces), elm.Formatting);
                        strApplied += 
                            ApplyFormatRules(Escape(_limitComment, replaces), Formatting.Bold);
                    }

                    break;
                }

                builder.Append(strApplied);
            };

            return builder.ToString();

            string ApplyFormatRules(string text, Formatting formatting)
            {
                if (formatting.HasFlag(Formatting.Bold)) text = boldFormatter(text);
                if (formatting.HasFlag(Formatting.Italic) && italicFormatter != null)
                    text = italicFormatter(text);
                if (formatting.HasFlag(Formatting.Underline) && underlineFormatter != null)
                    text = underlineFormatter(text);
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

    [Flags] //mb make hierarchy?
    public enum Formatting
    {
        None = 0,
        Bold = 1,
        Italic = 2,
        Underline = 4
    }
}
