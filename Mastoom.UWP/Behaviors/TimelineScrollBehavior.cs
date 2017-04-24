using Mastoom.Shared.Models.Mastodon.Status;
using Microsoft.Xaml.Interactivity;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Mastoom.UWP.Behaviors
{
    /// <summary>
    /// Manage timeline scrolling
    /// </summary>
	class TimelineScrollBehavior : Behavior<ScrollViewer>
	{
		private ScrollViewer attached;
		private double lastHeight = 0;
        private double lastScrollOffsetFromBottom = 0;
        private bool isPrevPage = false;
        private bool isNextPage = false;
        private bool isNextPage_Completed = false;

        // 連続操作をブロック
        private bool isContinuityBlocked;

        #region 依存プロパティ

        /// <summary>
        /// Collection which displaying in the timeline
        /// </summary>
        public MastodonStatusCollection Collection
        {
            get { return (MastodonStatusCollection)this.GetValue(CollectionProperty); }
            set { this.SetValue(CollectionProperty, value); }
        }
        public static readonly DependencyProperty CollectionProperty =
            DependencyProperty.RegisterAttached(
                nameof(Collection),
                typeof(MastodonStatusCollection),
                typeof(TimelineScrollBehavior),
                new PropertyMetadata(null, (sender, e) =>
                {
                    var b = sender as TimelineScrollBehavior;
                    if (b != null)
                    {
                        if (e.OldValue != null)
                        {
                            var collection = (MastodonStatusCollection)e.OldValue;
                            collection.PageModeExited -= b.Collection_PageModeExited;
                        }
                        if (e.NewValue != null)
                        {
                            var collection = (MastodonStatusCollection)e.NewValue;
                            collection.PageModeExited += b.Collection_PageModeExited;
                        }
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
                this.isContinuityBlocked = false;
            }
            else if (this.isNextPage)
            {
                this.isNextPage = false;
                this.isNextPage_Completed = true;

                this.lastScrollOffsetFromBottom = scrollLengthMax - scrollLength;
                this.Collection.NextPage();
            }
            else if (this.isNextPage_Completed)
            {
                this.isNextPage_Completed = false;

                this.attached.ChangeView(null, scrollLengthMax - this.lastScrollOffsetFromBottom, null, true);
                this.isContinuityBlocked = false;
            }

            this.lastHeight = scrollLengthMax;
        }

        private void Attached_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            if (this.isContinuityBlocked)
            {
                // 連続動作はブロック。操作と操作の間は少し待ってもらう
                return;
            }

            var scrollLength = this.attached.VerticalOffset;
            var scrollLengthMax = this.attached.ExtentHeight;
            var viewportHeight = this.attached.ViewportHeight;
            var collection = this.Collection;


            // 一番上へスクロールした状態
            if (scrollLength <= 10)
            {
                if (collection.PreviewPrevPage())
                {
                    this.isPrevPage = true;
                    this.isContinuityBlocked = true;
                }
                else
                {
                    collection.ExitPageMode();
                }
            }
            // 下へスクロールした状態
            else if (scrollLength >= scrollLengthMax - viewportHeight - 10)
            {
                if (collection.PreviewNextPage())
                {
                    this.isNextPage = true;
                    this.isContinuityBlocked = true;
                }
            }
            // 少しスクロールした状態
            else if (scrollLength > 10 && !collection.IsPageMode)
            {
                this.isNextPage = true;
                collection.EnterPageMode();
            }
        }

        private void Collection_PageModeExited(object sender, EventArgs e)
        {
            // Scroll to top (animation must be disabled, or cannot scroll well)
            this.attached.ChangeView(null, 0, null, true);
        }
    }
}
