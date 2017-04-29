using Mastoom.Shared.Models.Mastodon.Status;
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
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Mastoom.UWP.Behaviors
{
	class ImageCollectionBehavior : Behavior<StackPanel>
	{
		private StackPanel attached;

		#region 依存プロパティ

		/// <summary>
		/// 表示するコンテンツ内容
		/// </summary>
		public IEnumerable<MastodonAttachment> Attachments
		{
			get
			{
				return (IEnumerable<MastodonAttachment>)this.GetValue(AttachmentsProperty);
			}
			set
			{
				this.SetValue(AttachmentsProperty, value);
			}
		}
		public static readonly DependencyProperty AttachmentsProperty =
			DependencyProperty.RegisterAttached(
                "Attachments",
				typeof(IEnumerable<MastodonAttachment>),
				typeof(ImageCollectionBehavior),
				new PropertyMetadata(null, (s, e) =>
				{
					var view = s as ImageCollectionBehavior;
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

			var images = this.attached.Children;
			images.Clear();

            if (this.Attachments == null)
            {
                return;
            }

            foreach (var item in this.Attachments)
            {
                try
                {
                    images.Add(new Image
                    {
                        MaxHeight = 100,
                        Margin = new Thickness(0, 0, 8, 0),
                        Source = new BitmapImage(new Uri(item.PreviewUrl)),
                    });
                }
                catch
                {
                    images.Add(new TextBlock
                    {
                        Foreground = new SolidColorBrush(Colors.Red),
                        Text = "Error",
                        Margin = new Thickness(0, 0, 8, 0),
                    });
                }
            }

			// ついでに表示非表示を決める
			this.attached.Visibility = images.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
		}
	}
}
