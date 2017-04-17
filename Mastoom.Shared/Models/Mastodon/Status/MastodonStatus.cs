using Mastoom.Shared.Models.Mastodon.Account;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace Mastoom.Shared.Models.Mastodon.Status
{
	/// <summary>
	/// Mastodonのステータス
	/// </summary>
    public class MastodonStatus : INotifyPropertyChanged
    {
		/// <summary>
		/// ステータスのID
		/// </summary>
		public int Id { get; }

		/// <summary>
		/// アカウント
		/// </summary>
		public MastodonAccount Account { get; }

		/// <summary>
		/// ステータスの内容
		/// </summary>
		public string Content
		{
			get
			{
				return this._content;
			}
			set
			{
				if (this._content != value)
				{
					this._content = value;
					this.OnPropertyChanged();
				}
			}
		}
		private string _content;

		public MastodonStatus(int id, MastodonAccount account)
		{
			this.Id = id;
			this.Account = account;
		}

        private static int a = 0;
		public MastodonStatus(Mastonet.Entities.Status status)
		{
			this.Id = status.Id;
			this.Content = status.Content;
			this.Account = new MastodonAccount(status.Account);
		}

		public void CopyTo(MastodonStatus to)
		{
			if (to.Id != this.Id)
			{
				throw new InvalidOperationException("コピーしようとするStatusのIDが違います");
			}
			to.Content = this.Content;
		}

		#region INotifyPropertyChanged

		public event PropertyChangedEventHandler PropertyChanged;
		protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		#endregion

	}
}
