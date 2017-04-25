using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using CenterCLR.Sgml;
using Mastonet.Entities;
using Mastoom.Shared.Models.Mastodon.Status;

namespace Mastoom.Shared.Parsers
{
	public sealed class StatusParser
	{
		public IEnumerable<TootSpan> Parse(MastodonStatus status)
		{
			var content = status.Content;
			var encoding = Encoding.UTF8;
			var parsedSpans = new List<TootSpan>();

			// なんか一応 div で囲ってみてる(今のところ <p></p> で囲まれてるtootしかみたことないけど)
			using (var stream = new MemoryStream(encoding.GetBytes("<div>" + content.Replace("<br>", "<br/>") + "</div>")))
			using (var reader = new SgmlReader(stream))
			{
				XDocument doc = XDocument.Load(reader);

				//Console.WriteLine("start----");
				//Console.WriteLine("raw--");
				//Console.WriteLine(content);
				//Console.WriteLine("--raw");

				// 最初に見つかった XText の Node の親の Node群を列挙する → いきなり @amay とかで始まるとだめだー
				//var firstTextNode = doc.DescendantNodes().FirstOrDefault(node => node is XText);
				//var contentRoot = firstTextNode?.Parent?.Nodes()

				// しょうがないので決め打ち;(SgmlReader にしたら勝手に <html></html> が付いた)
				var contentRoot = doc?.Element("html")?.Element("div")?.Element("p") ?? null;
				foreach (var node in contentRoot?.Nodes() ?? new XNode[0])
				{
					if (node.NodeType == XmlNodeType.Text)
					{
						var xText = node as XText;
						parsedSpans.Add(TootSpan.MakeText(xText.Value));
						//Console.WriteLine($"text - {xText.Value}");
					}
					else if (node.NodeType == XmlNodeType.Element)
					{
						var element = node as XElement;
						switch (element.Name.LocalName.ToLower())
						{
							case "a":
								{
									var span = ParseLink(element, status.MediaAttachments, status.Tags);
									parsedSpans.Add(span);
									break;
								}
							case "br":
								{
									parsedSpans.Add(TootSpan.MakeLineBreak());
									break;
								}
							case "span":
								{
									// なんか @amay みたいなメンションは span になるらしいが判別できないのでただの Text とする
									parsedSpans.Add(TootSpan.MakeText(element.Value));
									break;
								}
							default:
								{
									//Console.WriteLine("Unknown Element: [" + element.Name + "]" + element.Value);
									throw new NotSupportedException($"Unknown Element: [{element.Name}]{element.Value} - {status.Id}");
									break;
								}
						}
					}
					else
					{
						// Unknown XNode
						//Console.WriteLine($"Unknown Node: {node}");
						throw new NotSupportedException($"Unknown Node: {node} - {status.Id}");
					}
				}
				//Console.WriteLine("----end");
			}

			return parsedSpans;
		}

		private TootSpan ParseLink(XElement element, IEnumerable<MastodonAttachment> mediaAttachments, IEnumerable<MastodonTag> tags)
		{
			var link = element.Attribute("href").Value;

			// mediaAttachments に含まれる URL なら何も表示しない
			// TODO Remote Instanse の画像は URL が一致しなくてここで認識できないみたいだけどとりあえず無視
			if (mediaAttachments != null && mediaAttachments.Any(arg => string.Compare(arg.TextUrl, link, StringComparison.Ordinal) == 0))
			{
				return TootSpan.MakeMedia();
			}

			// tags に含まれる URL なら tags名だけ表示する
			var tag = tags?.FirstOrDefault(arg => string.Compare(arg.Url, link, StringComparison.Ordinal) == 0) ?? null;
			if (tag != null)
			{
				return TootSpan.MakeTag(tag.Name);
			}

			// class="invisible" な子ノードを(複数)探して、得られた文字列 link から削除する
			// なんで class="ellipsis" が無いんだ？

			var invisibleWords = element
				.Nodes() // 子ノード群を
	            .OfType<XElement>() // XElement だけにして
	            .Where(elm => // class="invisible" で値も入っているものだけに絞って
				{
					// 属性値が空文字は要らない
						if (string.IsNullOrEmpty(elm.Value))
					{
						return false;
					}

					// class属性の値がinvisibleなものがあったらtrue
					var attrs = elm.Attributes("class");
					return attrs?.Any(a => string.Compare(a.Value, "invisible", StringComparison.Ordinal) == 0) ?? false;
				})
				.Select(e => e.Value); // 属性値を得る

			// 非表示ワード群をリンクから削ってく
			var displayLink = invisibleWords.Any() ? invisibleWords.Aggregate(link, (l, invisible) => l.Replace(invisible, "")) : link;

			return TootSpan.MakeHyperLink(displayLink, link);
		}
	}
}