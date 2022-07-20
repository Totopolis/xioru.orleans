using System.Text;

namespace Xioru.Messaging.Contracts.Formatting
{
    public class FormattedString
    {
        public FormattedString(string str, Formatting formatting = Formatting.None)
        {
            _formattedElements = new List<FormattingElement> { new FormattingElement(str, formatting) };
        }

        public FormattedString()
        {
            _formattedElements = new List<FormattingElement>();
        }

        private List<FormattingElement> _formattedElements;

        public FormattedString Append(string text, Formatting formatting = Formatting.None)
        {
            _formattedElements.Add(new FormattingElement(text, formatting));
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
            Dictionary<string, string>? replaces = null,
            Func<string, string>? boldFormatter = null,
            Func<string, string>? italicFormatter = null,
            Func<string, string>? underlineFormatter = null
            )
        {
            var sb = new StringBuilder();
            _formattedElements.ForEach(elm =>
            {
                if (replaces != null)
                {
                    elm.Escape(replaces);
                }

                var elmString = elm.ToString();
                if (elm.Formatting.HasFlag(Formatting.Bold) && boldFormatter != null) elmString = boldFormatter(elmString);
                if (elm.Formatting.HasFlag(Formatting.Italic) && italicFormatter != null) elmString = italicFormatter(elmString);
                if (elm.Formatting.HasFlag(Formatting.Underline) && underlineFormatter != null) elmString = underlineFormatter(elmString);
                
                sb.Append(elmString);
            });

            return sb.ToString();
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

    internal class FormattingElement
    {
        private string _text;
        public Formatting Formatting { get; }

        public FormattingElement(string text, Formatting formatting = Formatting.None)
        {
            _text = text;
            Formatting = formatting;
        }

        public void Escape(Dictionary<string, string> replaces)
        {
            foreach (var c in replaces.Keys)
            {
                _text = _text.Replace(c, replaces[c]);
            }
        }

        public override string ToString()
        {
            return _text;
        }
    }

    [Flags]
    public enum Formatting
    {
        None = 0,
        Bold = 1,
        Italic = 2,
        Underline = 4
    }
}
