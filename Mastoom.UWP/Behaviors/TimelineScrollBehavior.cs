using Mastoom.Shared.Models.Mastodon.Status;
using Microsoft.Xaml.Interactivity;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Mastoom.UWP.Behaviors
{
	class TimelineScrollBehavior : Behavior<ScrollViewer>
	{
		private ScrollViewer attached;
		private double lastHeight = 0;
        private double lastScrollOffsetFromBottom = 0;
        private bool isPrevPage = false;
        private bool isNextPage = false;
        
		public MastodonStatusCollection Collection
        {
            get
            {
                return (MastodonStatusCollection)this.GetValue(CollectionProperty);
            }
            set
            {
                this.SetValue(CollectionProperty, value);
            }
        }
        public static readonly DependencyProperty CollectionProperty =
            DependencyProperty.RegisterAttached(
                "Collection",
                typeof(MastodonStatusCollection),
                typeof(TimelineScrollBehavior),
                new PropertyMetadata(null)
            );

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
                this.attached.ViewChanged += this.Attached_ViewChanged;

				var itemsControl = (ItemsControl)this.attached.Content;
				itemsControl.LayoutUpdated += this.Content_HeightChanged;
			}
		}

        private void DetachObject()
		{
			if (this.attached != null)
			{
				var itemsControl = (ItemsControl)this.attached.Content;
				itemsControl.LayoutUpdated -= this.Content_HeightChanged;

                this.attached.ViewChanged -= this.Attached_ViewChanged;
				this.attached = null;
			}
		}

		private void Content_HeightChanged(object sender, object e)
        {
            var scrollLength = this.attached.VerticalOffset;
            var scrollLengthMax = this.attached.ExtentHeight;
            var viewportHeight = this.attached.ViewportHeight;

            if (this.isPrevPage)
            {
                this.isPrevPage = false;
                this.Collection.PrevPage();

                this.attached.ChangeView(null, scrollLength + (scrollLengthMax - this.lastHeight), null, true);
            }
            else if (this.isNextPage)
            {
                this.isNextPage = false;
                this.Collection.NextPage();

                this.attached.ChangeView(null, scrollLengthMax - this.lastScrollOffsetFromBottom + 30, null, true);
            }

            this.lastHeight = scrollLengthMax;
            this.lastScrollOffsetFromBottom = scrollLengthMax - scrollLength;
        }

        private void Attached_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            var scrollLength = this.attached.VerticalOffset;
            var scrollLengthMax = this.attached.ExtentHeight;
            var viewportHeight = this.attached.ViewportHeight;
            var collection = this.Collection;

            // 一番上へスクロールした状態
            if (scrollLength <= 10)
            {
                if (collection.PerformPrevPage())
                {
                    this.isPrevPage = true;
                }
                else
                {
                    collection.ExitPageMode();
                }
            }
            // 下へスクロールした状態
            else if (scrollLength >= scrollLengthMax - viewportHeight - 10)
            {
                this.isNextPage = true;
                collection.PerformNextPage();
            }
            // 少しスクロールした状態
            else if (scrollLength > 10 && !collection.IsPageMode)
            {
                this.isNextPage = true;
                collection.EnterPageMode();
            }
        }
    }
}
