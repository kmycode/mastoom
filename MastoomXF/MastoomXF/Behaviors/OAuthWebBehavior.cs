using System;
using Mastoom.Shared.Models.Common;
using Xamarin.Forms;

namespace Mastoom.Behaviors
{
	public class OAuthWebBehavior : Behavior<WebView>
	{
		private WebView associatedObject;

		#region 依存プロパティ

		public static readonly BindableProperty HelperProperty =
			BindableProperty.Create(
			"Helper",
			typeof(OAuthModel),
			typeof(OAuthWebBehavior),
			(OAuthModel)null,
			propertyChanged: OnHelperChanged);

		/// <summary>
		/// ブラウザ操作を命令するヘルパ
		/// </summary>
		public OAuthModel Helper
		{
			get
			{
				var obj = GetValue(HelperProperty);
				return (OAuthModel)obj;
			}
			set
			{
				SetValue(HelperProperty, value);
			}
		}

		static void OnHelperChanged(BindableObject bindable, object oldValue, object newValue)
		{
			var view = bindable as OAuthWebBehavior;
			if (view != null)
			{
				if (oldValue != null)
				{
					var authModel = (OAuthModel)oldValue;
					authModel.UriNavigateRequested -= view.Helper_NavigateRequested;
					authModel.HideRequested -= view.Helper_HideRequested;
					authModel.ShowRequested -= view.Helper_ShowRequested;;
				}
				if (newValue != null)
				{
					var authModel = (OAuthModel)newValue;
					authModel.UriNavigateRequested += view.Helper_NavigateRequested;
					authModel.ShowRequested += view.Helper_ShowRequested;;
					authModel.HideRequested += view.Helper_HideRequested;
					authModel.OnAttached();
				}
			}
		}

		#endregion

		protected override void OnAttachedTo(WebView bindable)
		{
			base.OnAttachedTo(bindable);

			associatedObject = bindable;

			if (bindable.BindingContext != null)
			{
				BindingContext = bindable.BindingContext;
			}
			bindable.BindingContextChanged += OnBindingContextChanged;

			associatedObject.Navigated += this.WebView_Navigated;
		}

		protected override void OnDetachingFrom(WebView bindable)
		{
			associatedObject.Navigated -= this.WebView_Navigated;

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

		private void WebView_Navigated(object sender, WebNavigatedEventArgs e)
		{
			this.Helper?.Navigated(e.Url);
		}

		private void Helper_NavigateRequested(object sender, UriNavigateRequestedEventArgs e)
		{
			try
			{
				if (this.associatedObject != null)
				{
					this.associatedObject.Source = new Uri(e.Uri);
				}
			}
			catch { }
		}

		private void Helper_ShowRequested(object sender, EventArgs e)
		{
			this.associatedObject.IsVisible = true;
		}

		private void Helper_HideRequested(object sender, EventArgs e)
		{
			this.associatedObject.IsVisible = false;
		}
	}
}
