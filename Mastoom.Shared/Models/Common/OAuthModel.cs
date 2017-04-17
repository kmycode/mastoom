using System;
using System.Collections.Generic;
using System.Text;

namespace Mastoom.Shared.Models.Common
{
    public class OAuthModel
    {
		public void NavigateRequest(string uri)
		{
			this.UriNavigateRequested?.Invoke(this, new UriNavigateRequestedEventArgs(uri));
		}

		public void Navigated(string uri)
		{
			this.UriNavigated?.Invoke(this, new UriNavigatedEventArgs(uri));
		}

		public void Hide()
		{
			this.HiddenRequested?.Invoke(this, new EventArgs());
		}

		public void OnAttached()
		{
			this.Attached?.Invoke(this, new EventArgs());
		}

		public event UriNavigateRequestedEventHandler UriNavigateRequested;
		public event UriNavigatedEventHandler UriNavigated;
		public event EventHandler HiddenRequested;
		public event EventHandler Attached;
    }

	public delegate void UriNavigatedEventHandler(object sender, UriNavigatedEventArgs e);
	public class UriNavigatedEventArgs : EventArgs
	{
		public string Uri { get; }
		internal UriNavigatedEventArgs(string uri)
		{
			this.Uri = uri;
		}
	}

	public delegate void UriNavigateRequestedEventHandler(object sender, UriNavigateRequestedEventArgs e);
	public class UriNavigateRequestedEventArgs : EventArgs
	{
		public string Uri { get; }
		internal UriNavigateRequestedEventArgs(string uri)
		{
			this.Uri = uri;
		}
	}
}
