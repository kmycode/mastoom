using Mastonet;
using Mastoom.Shared.Common;
using Mastoom.Shared.Models.Mastodon.Account;
using Mastoom.Shared.Models.Mastodon.Generic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace Mastoom.Shared.Models.Mastodon.Status
{
	/// <summary>
	/// Mastodonのステータス
	/// </summary>
    public class MastodonStatus : MastodonObject, INotifyPropertyChanged
    {
		/// <summary>
		/// アカウント
		/// </summary>
		public MastodonAccount Account { get; }

        /// <summary>
        /// これはブーストされた書き込みであるか
        /// </summary>
        public bool IsBoost { get; }

        /// <summary>
        /// ブーストされたトゥートのID
        /// </summary>
        public int BoostedId { get; }

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
        /// 自分がこの書き込みをブーストしたか
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

		/// <summary>
		/// 添付メディア(画像など)群
		/// </summary>
		/// <value>The media attachments.</value>
		public IEnumerable<MastodonAttachment> MediaAttachments
		{
			get { return _mediaAttachments; }
			private set
			{
				_mediaAttachments = value;
				this.OnPropertyChanged();
			}
		}
		private IEnumerable<MastodonAttachment> _mediaAttachments;

		/// <summary>
		/// タグ群
		/// </summary>
		/// <value>The tags.</value>
		public IEnumerable<MastodonTag> Tags
		{
			get { return _tags; }
			private set
			{
				_tags = value;
				this.OnPropertyChanged();
			}
		}
		private IEnumerable<MastodonTag> _tags;

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

        #region メソッド

        public MastodonStatus(int id, MastodonAccount account) : base(id)
		{
			this.Account = account;
		}

        internal MastodonStatus(int id, int boostedId, string content, bool isFavorited, 
                                bool isBoosted, MastodonAccount account, 
                                IEnumerable<MastodonAttachment> mediaAttachments, IEnumerable<MastodonTag> tags) : this(id, account)
        {
            this.BoostedId = boostedId;
            this.IsBoost = isBoosted;
        	this.Content = content;
        	this.IsFavorited = isFavorited;
        	this.IsBoosted = isBoosted;
            this.Account = account;
            this.MediaAttachments = mediaAttachments;
            this.Tags = tags;
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
            to.MediaAttachments = this.MediaAttachments.Select(x => x); // なんとなくIEnumerableのガワだけ生成しなおし
            to.Tags = this.Tags.Select(x => x); // なんとなくIEnumerableのガワだけ生成しなおし
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
                var newStatus = 
                    (!this.IsFavorited ? await client.Favourite(this.Id) : await client.Unfavourite(this.Id)).ToMastodonStatus();
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
                var newStatus = 
                    (!this.IsBoosted ? await client.Reblog(this.Id) : await client.Unreblog(this.Id)).ToMastodonStatus();
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
