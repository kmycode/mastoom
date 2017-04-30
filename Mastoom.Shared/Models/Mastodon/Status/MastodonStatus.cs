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
        /// 自分の発言であるか
        /// </summary>
        public bool IsMyStatus
        {
            get
            {
                return this._isMyStatus;
            }
            set
            {
                if (this._isMyStatus != value)
                {
                    this._isMyStatus = value;
                    this.OnPropertyChanged();
                }
            }
        }
        private bool _isMyStatus;

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
        /// ブーストしたトゥート
        /// </summary>
        public MastodonStatus Boost { get; }

        /// <summary>
        /// リプライであるか
        /// </summary>
        public bool IsReply { get; }

        /// <summary>
        /// リプライ先のStatusのID
        /// </summary>
        public int ReplyToId { get; }

        /// <summary>
        /// リプライ先のアカウントのID
        /// </summary>
        public int ReplyToAccountId { get; }

		/// <summary>
		/// 添付メディア(画像など)群
		/// </summary>
		/// <value>The media attachments.</value>
		public IEnumerable<MastodonAttachment> MediaAttachments { get; }

		/// <summary>
		/// タグ群
		/// </summary>
		/// <value>The tags.</value>
		public IEnumerable<MastodonTag> Tags { get; }
        
        /// <summary>
        /// 投稿日時
        /// </summary>
		public DateTime CreatedAt { get; }

        #region メソッド

        internal MastodonStatus(int id, MastodonAccount account) : base(id)
		{
			this.Account = account;
		}

        internal MastodonStatus(int id, MastodonAccount account, string content,
                                bool reblogged, bool favorited, MastodonStatus reblog,
                                int? replyToId, int? replyToAccountId,
                                IEnumerable<MastodonAttachment> mediaAttachments, IEnumerable<MastodonTag> tags,
                                DateTime createdAt) : this(id, account)
        {
            if (reblog != null)
            {
                this.BoostedId = reblog.Id;
                this.IsBoost = true;
            }

        	this.Content = content;
        	this.IsFavorited = favorited;
        	this.IsBoosted = reblogged;
            this.Boost = reblog;
            this.Account = account;
            this.IsReply = replyToAccountId != null || replyToId != null;
            this.ReplyToId = replyToId ?? 0;
            this.ReplyToAccountId = replyToAccountId ?? 0;
            this.MediaAttachments = mediaAttachments;
            this.Tags = tags;
            this.CreatedAt = createdAt;
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

        /// <summary>
        /// 発言を削除する
        /// </summary>
        /// <param name="client">クライアント</param>
        /// <returns>成功したかどうか</returns>
        public async Task<bool> DeleteAsync(MastodonClient client)
        {
            bool isSucceed = false;
            try
            {
                await client.DeleteStatus(this.Id);
                isSucceed = true;
            }
            catch { }

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
