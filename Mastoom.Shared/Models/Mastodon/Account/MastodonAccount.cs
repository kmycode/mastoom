using Mastoom.Shared.Models.Mastodon.Generic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Mastoom.Shared.Models.Mastodon.Account
{
    public class MastodonAccount : MastodonObject, INotifyPropertyChanged
    {
        /// <summary>
        /// 実際に表示されるID
        /// </summary>
        public string DisplayId { get; }

        /// <summary>
        /// インスタンスURI
        /// </summary>
        public string InstanceUri { get; }

        /// <summary>
        /// インスタンスが同じであるか（このアカウントを取得した認証と同じインスタンスであるか）
        /// </summary>
        public bool IsLocal { get; }

		/// <summary>
		/// 名前
		/// </summary>
		public string Name
		{
			get
			{
				return this._name;
			}
			set
			{
				if (this._name != value)
				{
					this._name = value;
					this.OnPropertyChanged();
				}
			}
		}
		private string _name;

		/// <summary>
		/// アイコンのURI
		/// </summary>
		public string IconUri
		{
			get
			{
				return this._iconUri;
			}
			set
			{
				if (this._iconUri != value)
				{
					this._iconUri = value;
					this.OnPropertyChanged();
				}
			}
		}
		private string _iconUri;

		public MastodonAccount(int id) : base(id)
		{
		}

		public MastodonAccount(Mastonet.Entities.Account account) : base(account.Id)
		{
            this.DisplayId = account.AccountName;
            this.InstanceUri = account.AccountName.Split('@').ElementAtOrDefault(1) ??
                               account.ProfileUrl.Split('/').ElementAt(2);
            this.IsLocal = !account.AccountName.Contains('@');
			this.Name = account.DisplayName;
			this.IconUri = account.AvatarUrl;
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
