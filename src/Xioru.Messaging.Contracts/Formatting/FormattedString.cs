using System.Text;

namespace Xioru.Messaging.Contracts.Formatting
{
    public class FormattedString
    {
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

        public IReadOnlyList<string> ToStringBatch(
            Dictionary<string, string>? replaces = null,
            Func<string, string>? boldFormatter = null,
            Func<string, string>? italicFormatter = null,
            Func<string, string>? underlineFormatter = null,
            int limit = int.MaxValue)
        {
            var sb = new StringBuilder();
            var messageBuilders = new List<StringBuilder> { sb };
            foreach(var elm in _formattedElements)
            {
                WrapElement(elm.Text, elm.Formatting);
            };

            return messageBuilders.Select(sb => sb.ToString()).ToList();

            void WrapElement(string rawText, Formatting formatting)
            {
                var escapedString = Escape(rawText, replaces);
                var strWithAppliedRules = ApplyFormatRules(escapedString, formatting);
                if (sb.Length + strWithAppliedRules.Length > limit)
                {
                    var serviceAdditionLength = strWithAppliedRules.Length - rawText.Length;
                    var rawLength = limit - sb.Length - serviceAdditionLength;
                    if (rawLength > 0)
                    {
                        WrapElement(rawText.Substring(0, rawLength), formatting);
                    }
                    else 
                    {
                        rawLength = 0; 
                    }

                    sb = new StringBuilder();
                    messageBuilders.Add(sb);
                    WrapElement(rawText.Substring(rawLength), formatting);
                }
                else
                {
                    sb.Append(strWithAppliedRules);
                }
            }

            string ApplyFormatRules(string text, Formatting formatting)
            {
                if (formatting.HasFlag(Formatting.Bold) && boldFormatter != null)
                    text = boldFormatter(text);
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
