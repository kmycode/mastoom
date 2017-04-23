using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using CenterCLR.Sgml;
using Mastoom.Shared.Models.Mastodon.Status;
using Mastoom.Shared.Parsers;
using Mastoom.Shared.Converters;
using Xamarin.Forms;

namespace Mastoom.Behaviors
{
	public class HtmlTextBehavior : Behavior<Label>
	{
		private Label associatedObject;
		private readonly String2EmojiConverter _emojiConverter = new String2EmojiConverter();

		#region 依存プロパティ

		public static readonly BindableProperty StatusProperty =
			BindableProperty.Create(
				"Status",
				typeof(MastodonStatus),
				typeof(HtmlTextBehavior),
				(MastodonStatus)null,
				propertyChanged: OnStatusChanged);

		/// <summary>
		/// ブラウザ操作を命令するヘルパ
		/// </summary>
		public MastodonStatus Status
		{
			get
			{
				var obj = GetValue(StatusProperty);
				return (MastodonStatus)obj;
			}
			set
			{
				SetValue(StatusProperty, value);
			}
		}

		static void OnStatusChanged(BindableObject bindable, object oldValue, object newValue)
		{

			var view = bindable as HtmlTextBehavior;
			view?.UpdateStatus();
		}

		#endregion

		protected override void OnAttachedTo(Label bindable)
		{
			base.OnAttachedTo(bindable);

			associatedObject = bindable;

			if (bindable.BindingContext != null)
			{
				BindingContext = bindable.BindingContext;
			}
			bindable.BindingContextChanged += OnBindingContextChanged;
		}

		protected override void OnDetachingFrom(Label bindable)
		{
			bindable.BindingContextChanged -= OnBindingContextChanged;

			associatedObject = null;

			base.OnDetachingFrom(bindable);
		}

		protected override void OnBindingContextChanged()
		{
			base.OnBindingContextChanged();

			BindingContext = associatedObject.BindingContext;
		}

		private void OnBindingContextChanged(object sender, EventArgs e)
		{
			OnBindingContextChanged();
		}

		private void UpdateStatus()
		{
			if (this.associatedObject == null)
			{
				return;
			}

			var formattedString = new FormattedString();
			var inlines = formattedString.Spans;

			var parser = new StatusParser();
			var tootSpans = parser.Parse(this.Status);

			foreach (var tootSpan in tootSpans)
			{
				var span = new Span { Text = tootSpan.Text };
				switch (tootSpan.Type)
				{
					case TootSpan.SpanType.Text:
						// 絵文字変換を忘れない
						span.Text = (string)_emojiConverter.Convert(tootSpan.Text, typeof(string), null, null);
						inlines.Add(span);
						break;
					case TootSpan.SpanType.HyperLink:
						span.ForegroundColor = Color.Blue;
						inlines.Add(span);
						break;
					case TootSpan.SpanType.Tag:
						span.ForegroundColor = Color.Blue;
						inlines.Add(span);
						break;
					case TootSpan.SpanType.LineBreak:
						span.Text = Environment.NewLine;
						inlines.Add(span);
						break;
					default:
						break;
				}

			}

			this.associatedObject.FormattedText = formattedString;

			// ついでに表示非表示を決める
			this.associatedObject.IsVisible = true; // inlines.Count > 0;
		}
	}
}
