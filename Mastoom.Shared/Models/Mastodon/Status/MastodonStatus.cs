using Mastoom.Shared.Models.Mastodon.Account;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using Mastoom.Shared.Parsers;
using System.Linq;

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

		private IEnumerable<MastodonAttachment> _mediaAttachments;
		public IEnumerable<MastodonAttachment> MediaAttachments
		{
			get { return _mediaAttachments; }
			private set
			{
				_mediaAttachments = value;
				this.OnPropertyChanged();
			}
		}

		private IEnumerable<MastodonTag> _tags;
		public IEnumerable<MastodonTag> Tags
		{
			get { return _tags; }
			private set
			{
				_tags = value;
				this.OnPropertyChanged();
			}
		}

		private DateTime _createdAt;
		public DateTime CreatedAt
		{
			get { return _createdAt; }
			private set
			{
				if (_createdAt == value)
				{
					return;
				}
				_createdAt = value;
				this.OnPropertyChanged();
			}
		}

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
			this.MediaAttachments = status.MediaAttachments.Select(x => new MastodonAttachment(x));
			this.Tags = status.Tags.Select(x => new MastodonTag(x));
			this.CreatedAt = status.CreatedAt;
		}

		public void CopyTo(MastodonStatus to)
		{
			if (to.Id != this.Id)
			{
				throw new InvalidOperationException("コピーしようとするStatusのIDが違います");
			}
			to.Content = this.Content;
			to.MediaAttachments = this.MediaAttachments.Select(x => x);
			to.Tags = this.Tags.Select(x => x);
			to.CreatedAt = this.CreatedAt;
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
