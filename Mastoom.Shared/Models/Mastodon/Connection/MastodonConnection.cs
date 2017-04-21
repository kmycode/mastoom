using Mastonet;
using Mastonet.Entities;
using Mastoom.Shared.Common;
using Mastoom.Shared.Models.Common;
using Mastoom.Shared.Models.Mastodon.Account;
using Mastoom.Shared.Models.Mastodon.Connection;
using Mastoom.Shared.Models.Mastodon.Connection.Function;
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
        
        private IConnectionFunction function;

        private ConnectionFunctionType functionType;

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

		public MastodonConnection(string instanceUri, ConnectionFunctionType functionType)
		{
            this.InstanceUri = instanceUri;
            this.functionType = functionType;
            this.Auth = MastodonAuthenticationHouse.Get(this.InstanceUri);

            if (!this.Auth.HasAuthenticated)
            {
                // 認証完了した時の処理
                this.Auth.Completed += (sender, e) =>
                {
                    this.ImportAuthenticationData();
                    Task.Run(async () =>
                    {
                        await this.StartFunctionAsync();
                    }).Wait();
                };

                // 認証開始
                this.Auth.StartOAuthLogin();
            }
            else
            {
                this.ImportAuthenticationData();
                Task.Run(async () =>
                {
                    await this.StartFunctionAsync();
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

        private async Task StartFunctionAsync()
        {
            if (this.function != null)
            {
                return;
            }

            switch (this.functionType)
            {
                case ConnectionFunctionType.PublicTimeline:
                    var func = await this.Auth.PublicStreamingFunctionCounter.IncrementAsync();
                    func.Updated += this.StatusFunction_OnUpdate;
                    this.function = func;
                    break;
            }

            await this.function.StartAsync();
        }

        private async Task StopFunctionAsync()
        {
            if (this.function == null)
            {
                return;
            }

            switch (this.functionType)
            {
                case ConnectionFunctionType.PublicTimeline:
                    var func = (IConnectionFunction<MastodonStatus>)this.function;
                    func.Updated -= this.StatusFunction_OnUpdate;
                    await this.Auth.PublicStreamingFunctionCounter.DecrementAsync();
                    break;
            }

            this.function = null;
        }

		private void StatusFunction_OnUpdate(object sender, ObjectFunctionEventArgs<MastodonStatus> e)
		{
			GuiThread.Run(() =>
			{
				this.Statuses.Add(e.Object);
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
