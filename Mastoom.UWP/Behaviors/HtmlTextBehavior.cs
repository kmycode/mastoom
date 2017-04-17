using Microsoft.Xaml.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
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

			var doc = new XmlDocument();
            try
            {
                doc.LoadXml("<div>" + this.Content.Replace("<br>", "<br/>") + "</div>");
            }
            catch
            {
                return;
            }

            var root = doc.HasChildNodes ? doc.FirstChild : null;
			if (root == null)
			{
				return;
			}

			foreach (XmlNode nodeAtRoot in root.ChildNodes)
			{
				foreach (XmlNode node in nodeAtRoot.ChildNodes)
				{
					if (node is XmlElement element)
					{
						switch (element.Name.ToLower())
						{
							case "a":
								if (element.HasAttribute("href"))
								{
									var link = element.Attributes["href"].Value;
									inlines.Add(new Hyperlink
									{
										NavigateUri = new Uri(element.Attributes["href"].Value),
										Inlines =
										{
											new Run
											{
												Text = this.GetPlainText(element),
											},
										},
									});
								}
								break;
							case "br":
								inlines.Add(new LineBreak());
								break;
							case "span":
								inlines.Add(new Run
								{
									Text = element.Value ?? "",
								});
								break;
							default:
								inlines.Add(new Run
								{
									Text = "[" + element.Name + "]" + element.Value,
									Foreground = new SolidColorBrush(Colors.Red),
								});
								break;
						}
					}
					else
					{
						inlines.Add(new Run
						{
							Text = node.Value,
						});
					}
				}
			}

			// ついでに表示非表示を決める
			this.attached.Visibility = inlines.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
		}

		private string GetPlainText(XmlElement element)
		{
			string value = "";
			foreach (XmlNode node in element)
			{
				if (node.HasChildNodes && node is XmlElement elem)
				{
					value += this.GetPlainText(elem);
				}
				else
				{
					value += node.Value;
				}
			}
			return value;
		}
	}
}
