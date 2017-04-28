using System;
namespace Mastoom.Shared.Parsers
{
    /// <summary>
    /// Toot の Contents をパースしてできた Span を示す
    /// </summary>
    /// <remarks>
    /// SpanType 毎にサブクラス作るべきかもだけど、今はいいや。
    /// </remarks>
    public sealed class TootSpan
    {
        SpanType media;
        string empty;

        public enum SpanType
        {
            None,
            Text,
            LineBreak,
            Tag,
            HyperLink,
            Media,
            //Mention とかあるかなー？
        }

        public SpanType Type { get; }

        public string Text { get; }

        public string Url { get; }

        private TootSpan(SpanType type, string text, string url)
        {
            this.Type = type;
            this.Text = text;
            this.Url = url;
        }

        internal static TootSpan MakeText(string text)
        {
            return new TootSpan(SpanType.Text, text, string.Empty);
        }

        internal static TootSpan MakeLineBreak()
        {
            return new TootSpan(SpanType.LineBreak, string.Empty, string.Empty);
        }

        internal static TootSpan MakeMedia()
        {
            return new TootSpan(SpanType.Media, string.Empty, string.Empty);
        }

        internal static TootSpan MakeTag(string tagName)
        {
            return new TootSpan(SpanType.Tag, $"#{tagName}", string.Empty);
        }

        internal static TootSpan MakeHyperLink(string text, string url)
        {
            return new TootSpan(SpanType.HyperLink, text, url);
        }

        public override string ToString()
        {
            var text = Text.Replace("{", "{{").Replace("}", "}}");
            return string.Format($"[TootSpan: Type={Type}, Text={text}, Url={Url}]");
        }
    }
}
