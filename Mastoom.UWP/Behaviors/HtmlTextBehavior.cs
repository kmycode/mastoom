using HtmlAgilityPack;
using Microsoft.Xaml.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;

namespace Mastoom.UWP.Behaviors
{
	/// <summary>
	/// テキストをHTML風テキストに変換する
	/// </summary>
	class HtmlTextBehavior : Behavior<TextBlock>
	{
		private TextBlock attached;

		#region 依存プロパティ

		/// <summary>
		/// 表示するコンテンツ内容
		/// </summary>
		public string Content
		{
			get
			{
				return (string)this.GetValue(ContentProperty);
			}
			set
			{
				this.SetValue(ContentProperty, value);
			}
		}
		public static readonly DependencyProperty ContentProperty =
			DependencyProperty.RegisterAttached(
				"Content",
				typeof(string),
				typeof(HtmlTextBehavior),
				new PropertyMetadata(null, (s, e) =>
				{
					var view = s as HtmlTextBehavior;
					if (view != null)
					{
						view.UpdateContent();
					}
				})
			);

		#endregion

		protected override void OnAttached()
		{
			base.OnAttached();
			this.AttachObject();
		}

		protected override void OnDetaching()
		{
			base.OnDetaching();
			this.DetachObject();
		}

		private void AttachObject()
		{
			this.DetachObject();

			if (this.AssociatedObject != null)
			{
				this.attached = this.AssociatedObject;
			}
		}

		private void DetachObject()
		{
			if (this.attached != null)
			{
				this.attached = null;
			}
		}

		private void UpdateContent()
		{
			if (this.attached == null)
			{
				return;
			}

			var inlines = this.attached.Inlines;
			inlines.Clear();

			var doc = new HtmlDocument();
            string html = WebUtility.HtmlDecode(this.Content);
            try
            {
                doc.LoadHtml(html);
            }
            catch (Exception e)
            {
                inlines.Add(new Run
                {
                    Text = this.Content,
                    Foreground = new SolidColorBrush(Colors.Red),
                });
                return;
            }

            var root = doc.DocumentNode;
			if (root == null)
			{
				return;
			}

			foreach (var nodeAtRoot in root.ChildNodes)
			{
				foreach (var node in nodeAtRoot.ChildNodes)
				{
					switch (node.Name.ToLower())
					{
						case "a":
							if (node.Attributes.Any(item => item.Name == "href"))
							{
								var link = node.Attributes["href"].Value;
								inlines.Add(new Hyperlink
								{
									NavigateUri = new Uri(node.Attributes["href"].Value),
									Inlines =
									{
										new Run
										{
											Text = node.InnerText,
										},
									},
								});
							}
							break;
						case "br":
							inlines.Add(new LineBreak());
							break;
						default:
							inlines.Add(new Run
							{
								Text = node.InnerText,
							});
							break;
					}
				}
			}

			// ついでに表示非表示を決める
			this.attached.Visibility = inlines.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
		}
	}
}
