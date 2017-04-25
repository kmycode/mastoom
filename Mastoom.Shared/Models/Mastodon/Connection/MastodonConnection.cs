using Mastonet;
using Mastonet.Entities;
using Mastoom.Shared.Common;
using Mastoom.Shared.Models.Common;
using Mastoom.Shared.Models.Mastodon.Account;
using Mastoom.Shared.Models.Mastodon.Connection;
using Mastoom.Shared.Models.Mastodon.Connection.Function;
using Mastoom.Shared.Models.Mastodon.Notification;
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
using static Mastoom.Shared.ViewModels.ViewModelBase;

namespace Mastoom.Shared.Models.Mastodon
{
    /// <summary>
    /// マストドンへのひとつの接続。
    /// １つの接続は１つのIConnectionFunctionを保持するため、
    /// 公開タイムラインへの接続や、お気に入り一覧への接続などは、別々の接続としてカウントされる
    /// </summary>
    public class MastodonConnection : INotifyPropertyChanged
    {
		#region 変数

		private const string ApplicationName = "Mastoom.Master.Test";
		
		private MastodonClient client;
        
		private string streamingInstance;
        
        private IConnectionFunction function;

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
        /// 接続の種類
        /// </summary>
        public ConnectionType ConnectionType => this._connectionType;
        private ConnectionType _connectionType;

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
        /// 通知の集まり
        /// </summary>
        public MastodonNotificationCollection Notifications { get; } = new MastodonNotificationCollection();

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

		public MastodonConnection(string instanceUri, ConnectionType functionType)
		{
            this.InstanceUri = instanceUri;
            this._connectionType = functionType;
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

            switch (this._connectionType)
            {
                case ConnectionType.PublicTimeline:
                    {
                        var func = await this.Auth.PublicStreamingFunctionCounter.IncrementAsync();
                        func.Updated += this.StatusFunction_OnUpdate;
                        this.function = func;
                    }
                    break;
                case ConnectionType.LocalTimeline:
                    {
                        var func = await this.Auth.PublicStreamingFunctionCounter.IncrementAsync();
                        func.Updated += this.StatusFunction_OnUpdate;
                        this.Statuses.Filter = (status) => status.Account.IsLocal;
                        this.function = func;
                    }
                    break;
                case ConnectionType.HomeTimeline:
                    {
                        var func = await this.Auth.HomeStreamingFunctionCounter.IncrementAsync();
                        func.Updated += this.StatusFunction_OnUpdate;
                        this.function = func;
                    }
                    break;
                case ConnectionType.Notification:
                    {
                        var func = await this.Auth.NotificationStreamingFunctionCounter.IncrementAsync();
                        func.Updated += this.NotificationFunction_OnUpdate;
                        this.function = func;
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }

            await this.function.StartAsync();
        }

        private async Task StopFunctionAsync()
        {
            if (this.function == null)
            {
                return;
            }

            switch (this._connectionType)
            {
                case ConnectionType.PublicTimeline:
                case ConnectionType.LocalTimeline:
                    {
                        var func = (IConnectionFunction<MastodonStatus>)this.function;
                        func.Updated -= this.StatusFunction_OnUpdate;
                        await this.Auth.PublicStreamingFunctionCounter.DecrementAsync();
                    }
                    break;
                case ConnectionType.HomeTimeline:
                    {
                        var func = (IConnectionFunction<MastodonStatus>)this.function;
                        func.Updated -= this.StatusFunction_OnUpdate;
                        await this.Auth.HomeStreamingFunctionCounter.DecrementAsync();
                    }
                    break;
                case ConnectionType.Notification:
                    {
                        var func = (IConnectionFunction<MastodonNotification>)this.function;
                        func.Updated -= this.NotificationFunction_OnUpdate;
                        await this.Auth.NotificationStreamingFunctionCounter.DecrementAsync();
                    }
                    break;
                default:
                    throw new NotImplementedException();
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

        private void NotificationFunction_OnUpdate(object sender, ObjectFunctionEventArgs<MastodonNotification> e)
        {
            GuiThread.Run(() =>
            {
                this.Notifications.Add(e.Object);
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

        #region コマンド

        // MastodonClientとMastodonStatusなど、複数オブジェクトの組み合わせを
        // 一度にViewModelに渡す方法が分からなかったので、
        // ここはModel層だけどICommandを実装したMVVM/Xamlに依存したクラスを使う

        /// <summary>
        /// ふぁぼる
        /// </summary>
        public RelayCommand<MastodonStatus> ToggleFavoriteCommand
        {
            get
            {
                return this._toggleFavoriteCommand = this._toggleFavoriteCommand ?? new RelayCommand<MastodonStatus>((status) =>
                {
                    status.ToggleFavoriteAsync(this.client);
                });
            }
        }
        private RelayCommand<MastodonStatus> _toggleFavoriteCommand;

        /// <summary>
        /// ブーストする
        /// </summary>
        public RelayCommand<MastodonStatus> ToggleBoostCommand
        {
            get
            {
                return this._toggleBoostCommand = this._toggleBoostCommand ?? new RelayCommand<MastodonStatus>((status) =>
                {
                    status.ToggleBoostAsync(this.client);
                });
            }
        }
        private RelayCommand<MastodonStatus> _toggleBoostCommand;

        #endregion
    }
}
