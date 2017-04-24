using Mastoom.Shared.Models.Common;
using Microsoft.Xaml.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Mastoom.UWP.Behaviors
{
	class OAuthWebBehavior : Behavior<WebView>
	{
		private WebView attached;

		#region 依存プロパティ

		/// <summary>
		/// ブラウザ操作を命令するヘルパ
		/// </summary>
		public OAuthModel Helper
		{
			get
			{
				return (OAuthModel)this.GetValue(HelperProperty);
			}
			set
			{
				this.SetValue(HelperProperty, value);
			}
		}
		public static readonly DependencyProperty HelperProperty =
			DependencyProperty.RegisterAttached(
				"Helper",
				typeof(OAuthModel),
				typeof(OAuthWebBehavior),
				new PropertyMetadata(null, (s, e) =>
				{
					var view = s as OAuthWebBehavior;
					if (view != null)
					{
						if (e.OldValue != null)
						{
							var oldValue = (OAuthModel)e.OldValue;
							oldValue.UriNavigateRequested -= view.Helper_NavigateRequested;
							oldValue.HideRequested -= view.Helper_HideRequested;
                            oldValue.ShowRequested -= view.Helper_ShowRequested;
                        }
						if (e.NewValue != null)
						{
							var newValue = (OAuthModel)e.NewValue;
							newValue.UriNavigateRequested += view.Helper_NavigateRequested;
							newValue.HideRequested += view.Helper_HideRequested;
                            newValue.ShowRequested += view.Helper_ShowRequested;
                            newValue.OnAttached();
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
				this.attached.NavigationCompleted += this.WebView_Navigated;
			}
		}

		private void DetachObject()
		{
			if (this.attached != null)
			{
				this.attached.NavigationCompleted -= this.WebView_Navigated;
				this.attached = null;
			}
		}

		private void WebView_Navigated(object sender, WebViewNavigationCompletedEventArgs e)
		{
			this.Helper?.Navigated(e.Uri.AbsoluteUri);
		}

		private void Helper_NavigateRequested(object sender, UriNavigateRequestedEventArgs e)
		{
			try
			{
				this.attached?.Navigate(new Uri(e.Uri));
			}
			catch { }
        }

        private void Helper_ShowRequested(object sender, EventArgs e)
        {
            this.attached.Visibility = Visibility.Visible;
        }

        private void Helper_HideRequested(object sender, EventArgs e)
		{
			this.attached.Visibility = Visibility.Collapsed;
		}
	}
}
