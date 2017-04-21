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
					authModel.HiddenRequested -= view.Helper_HiddenRequested;
				}
				if (newValue != null)
				{
					var authModel = (OAuthModel)newValue;
					authModel.UriNavigateRequested += view.Helper_NavigateRequested;
					authModel.HiddenRequested += view.Helper_HiddenRequested;
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

			//RegisterEvent(EventName);
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

		//protected override void OnAttachedTo(WebView bindable)
		//{
		//	base.OnAttachedTo(bindable);
		//	this.AttachObject(bindable);
		//}

		//protected override void OnDetachingFrom(BindableObject bindable)
		//{
 	//		bindable.BindingContextChanged -= OnBindingContextChanged;			
  //          this.DetachObject();
		//	base.OnDetachingFrom(bindable);
		//}

		//private void AttachObject(WebView bindable)
		//{
		//	this.DetachObject();
  //          this.attached = bindable;

		//	if (bindable != null)
		//	{
		//		bindable.Navigated += this.WebView_Navigated;
		//		bindable.Source = "http://yahoo.co.jp";

		//		if (bindable.BindingContext != null)
		//		{
		//            if (bindable.BindingContext != null)
		//            {
		//                BindingContext = bindable.BindingContext;
		//            }
		//            bindable.BindingContextChanged += OnBindingContextChanged;
		//		}
		//	}
		//}

		//private void DetachObject()
		//{
		//	if (this.attached != null)
		//	{
		//		this.attached.Navigated -= this.WebView_Navigated;
		//		this.attached = null;
		//	}
		//}

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

		private void Helper_HiddenRequested(object sender, EventArgs e)
		{
			this.associatedObject.IsVisible = false;
		}

		//protected override void OnBindingContextChanged()
		//{
		//	base.OnBindingContextChanged();

		//	BindingContext = attached.BindingContext;
		//}

		//private void OnBindingContextChanged(object sender, EventArgs e)
		//{
		//	OnBindingContextChanged();
		//}
	}
}
