using Mastonet;
using Mastonet.Entities;
using Mastoom.Shared.Common;
using Mastoom.Shared.Models.Common;
using Mastoom.Shared.Models.Mastodon.Account;
using Mastoom.Shared.Models.Mastodon.Connection;
using Mastoom.Shared.Models.Mastodon.Status;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mastoom.Shared.Models.Mastodon
{
    public class MastodonConnection : INotifyPropertyChanged
    {
		#region 変数

		private const string ApplicationName = "Mastoom.Master.Test";
		
		private MastodonClient client;

		private TimelineStreaming publicStatusStreaming;
        
		private string streamingInstance;
        private const int StreamingTimeOut = 10;    // 一定時間ストリーミングの更新がない時に接続し直す（バグ？）
        private Timer StreamingTimer;

		#endregion

		#region プロパティ

        /// <summary>
        /// マストドンのひとつの認証
        /// </summary>
        public MastodonAuthentication Auth { get; private set; }

		/// <summary>
		/// 接続の名前
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
		/// インスタンスのURI
		/// </summary>
		public string InstanceUri { get; }

        /// <summary>
        /// ログインしているユーザ
        /// </summary>
        public MastodonAccount Account
        {
            get
            {
                return this._account;
            }
            private set
            {
                if (this._account != value)
                {
                    this._account = value;
                    this.OnPropertyChanged();
                }
            }
        }
        private MastodonAccount _account;

		/// <summary>
		/// ステータスの集まり
		/// </summary>
		public MastodonStatusCollection Statuses { get; } = new MastodonStatusCollection();

		/// <summary>
		/// ステータスを投稿するモデル
		/// </summary>
		public PostStatusModel PostStatus
		{
			get
			{
				return this._postStatus;
			}
			set
			{
				this._postStatus = value;
				this.OnPropertyChanged();
			}
		}
		private PostStatusModel _postStatus;

		#endregion

		#region メソッド

		public MastodonConnection(string instanceUri)
		{
            this.InstanceUri = instanceUri;
            this.Auth = MastodonAuthenticationHouse.Get(this.InstanceUri);

            // ストリーミングの更新がタイムアウトした時の処理
            this.StreamingTimer = new Timer((sender) =>
            {
                this.StopStreamingPublicStatus();
                this.StartStreamingPublicStatus();
            }, null, Timeout.Infinite, Timeout.Infinite);

            if (!this.Auth.HasAuthenticated)
            {
                // 認証完了した時の処理
                this.Auth.Completed += (sender, e) =>
                {
                    this.ImportAuthenticationData();
                    Task.Run(() =>
                    {
                        this.StartStreamingPublicStatus();
                    }).Wait();
                };

                // 認証開始
                this.Auth.StartOAuthLogin();
            }
            else
            {
                this.ImportAuthenticationData();
                Task.Run(() =>
                {
                    this.StartStreamingPublicStatus();
                }).Wait();
            }
        }

        private void ImportAuthenticationData()
        {
            this.client = this.Auth.Client;
            this.streamingInstance = this.Auth.StreamingUri;
            this.PostStatus = new PostStatusModel(this.client);
            this.Account = this.Auth.CurrentUser;
        }

		private async Task GetNewerStatuses()
		{
			var timeline = await this.client.GetPublicTimeline();
			foreach (var item in timeline)
			{
				this.Statuses.Add(new MastodonStatus(item));
			}
		}

        private void StartStreamingPublicStatus()
        {
            this.StartStreamingPublicStatus(1);
        }

		private void StartStreamingPublicStatus(int tryCount)
		{
            // 試行回数
            if (tryCount > 3)
            {
                return;
            }

            try
            {
                if (this.publicStatusStreaming == null)
			    {
                    if (this.streamingInstance == null)
                    {
                        this.publicStatusStreaming = this.client.GetPublicStreaming();
                    }
                    else
                    {
                        this.publicStatusStreaming = this.client.GetPublicStreaming(this.streamingInstance);
                    }

                    this.publicStatusStreaming.OnUpdate += this.PublicStatus_OnUpdate;
                }

                this.StreamingTimer.Change(1000 * StreamingTimeOut, Timeout.Infinite);
                this.publicStatusStreaming.Start();
			}
			catch (Exception e)
			{
				this.StopStreamingPublicStatus();
				this.StartStreamingPublicStatus(tryCount + 1);
			}
		}

		private void StopStreamingPublicStatus()
		{
			if (this.publicStatusStreaming != null)
			{
				try
				{
					this.publicStatusStreaming.Stop();
				}
				catch { }
				this.publicStatusStreaming.OnUpdate -= this.PublicStatus_OnUpdate;
				this.publicStatusStreaming = null;
                this.StreamingTimer.Change(Timeout.Infinite, Timeout.Infinite);
			}
		}

		private void PublicStatus_OnUpdate(object sender, StreamUpdateEventArgs e)
		{
            this.StreamingTimer.Change(1000 * StreamingTimeOut, Timeout.Infinite);
			GuiThread.Run(() =>
			{
				this.Statuses.Add(new MastodonStatus(e.Status));
			});
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
