using Mastonet;
using Mastoom.Shared.Common;
using Mastoom.Shared.Models.Mastodon.Account;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

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

        /// <summary>
        /// ふぁぼったか
        /// </summary>
        public bool IsFavorited
        {
            get
            {
                return this._isFavorited;
            }
            private set
            {
                if (this._isFavorited != value)
                {
                    this._isFavorited = value;
                    this.OnPropertyChanged();
                }
            }
        }
        private bool _isFavorited;

        /// <summary>
        /// ブーストされたか
        /// </summary>
        public bool IsBoosted
        {
            get
            {
                return this._isBoosted;
            }
            private set
            {
                if (this._isBoosted != value)
                {
                    this._isBoosted = value;
                    this.OnPropertyChanged();
                }
            }
        }
        private bool _isBoosted;

        #region メソッド

        public MastodonStatus(int id, MastodonAccount account)
		{
			this.Id = id;
			this.Account = account;
		}
        
		public MastodonStatus(Mastonet.Entities.Status status)
		{
			this.Id = status.Id;
			this.Content = status.Content;
            this.IsFavorited = status.Favourited ?? false;
            this.IsBoosted = status.Reblogged ?? false;
			this.Account = new MastodonAccount(status.Account);
		}

		public void CopyTo(MastodonStatus to)
		{
			if (to.Id != this.Id)
			{
				throw new InvalidOperationException("コピーしようとするStatusのIDが違います");
			}
			to.Content = this.Content;
            to.IsFavorited = this.IsFavorited;
            to.IsBoosted = this.IsBoosted;
		}

        #endregion

        #region サーバと通信し状態を操作するメソッド

        /// <summary>
        /// ふぁぼとあんふぁぼを切り替える
        /// </summary>
        /// <param name="client">クライアント</param>
        /// <returns>成功したかどうか</returns>
        public async Task<bool> ToggleFavoriteAsync(MastodonClient client)
        {
            bool isSucceed = false;
            try
            {
                var newStatus = new MastodonStatus(
                    !this.IsFavorited ? await client.Favourite(this.Id) : await client.Unfavourite(this.Id)
                    );
                isSucceed = true;
            }
            catch { }

            if (isSucceed)
            {
                this.IsFavorited ^= true;
            }

            return isSucceed;
        }

        /// <summary>
        /// ブーストとアンブーストを切り替える
        /// </summary>
        /// <param name="client">クライアント</param>
        /// <returns>成功したかどうか</returns>
        public async Task<bool> ToggleBoostAsync(MastodonClient client)
        {
            bool isSucceed = false;
            try
            {
                var newStatus = new MastodonStatus(
                    !this.IsBoosted ? await client.Reblog(this.Id) : await client.Unreblog(this.Id)
                    );
                isSucceed = true;
            }
            catch { }

            if (isSucceed)
            {
                this.IsBoosted ^= true;
            }

            return isSucceed;
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
		protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		#endregion

	}
}
