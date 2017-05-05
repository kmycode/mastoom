using Mastonet;
using Mastonet.Entities;
using Mastoom.Shared.Common;
using Mastoom.Shared.Models.Common;
using Mastoom.Shared.Models.Mastodon.Account;
using Mastoom.Shared.Models.Mastodon.Connection;
using Mastoom.Shared.Models.Mastodon.Connection.Function;
using Mastoom.Shared.Models.Mastodon.Connection.Function.Container;
using Mastoom.Shared.Models.Mastodon.Generic;
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
using Mastoom.Shared.Repositories;

namespace Mastoom.Shared.Models.Mastodon
{
    /// <summary>
    /// マストドンへのひとつの接続。
    /// １つの接続は１つのIConnectionFunctionを保持するため、
    /// 公開タイムラインへの接続や、お気に入り一覧への接続などは、別々の接続としてカウントされる
    /// </summary>
    /// <remarks>
    /// 後で追加する機能に制約がかからないよう、型引数はあえてつけない
    /// </remarks>
    public class MastodonConnection : INotifyPropertyChanged
    {
		#region 変数

		private const string ApplicationName = "Mastoom.Master.Test";
		
		private MastodonClient client;
        
		private string streamingInstance;

        private IFunctionContainer container;

        private OAuthAccessTokenRepository tokenRepo;

        #endregion

        #region プロパティ

        /// <summary>
        /// マストドンのひとつの認証
        /// </summary>
        public MastodonAuthentication Auth { get; private set; }

        /// <summary>
        /// 接続の種類
        /// </summary>
        public ConnectionType ConnectionType => this.container.ConnectionType;

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
        /// タイムラインに表示するオブジェクト
        /// </summary>
        public ITimelineCollection TimelineObjects => this.container.Objects;

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

        /// <summary>
        /// 画面遷移を制御するモデル。
        /// これを利用して、トゥートの詳細などを表示する
        /// </summary>
        public ConnectionFrameStackModel FrameStack { get; private set; } = new ConnectionFrameStackModel();

		#endregion

		#region メソッド

        internal MastodonConnection(string instanceUri, IFunctionContainer functionContainer, OAuthAccessTokenRepository tokenRepo)
        {
            this.InstanceUri = instanceUri;
            this.container = functionContainer;
            this.tokenRepo = tokenRepo;

            this.InitializeAsync();
        }

        private void PushNewFrame(IFunctionContainer functionContainer)
        {
            var frame = new MastodonConnection(this.InstanceUri, functionContainer, this.tokenRepo)
            {
                FrameStack = this.FrameStack
            };
            this.FrameStack.Push(frame);
        }

        private async void InitializeAsync()
        {
            this.Auth = await MastodonAuthenticationHouse.Get(this.InstanceUri, this.tokenRepo);

            while (!this.Auth.HasAuthenticated)
            {
                await this.Auth.DoAuth(this.tokenRepo);

                // TODO AccessToken が無効になってた場合にのみここに来るはず。
                // AccessToken をクリアして、WebView で OAuth 認証からやり直す必要あり。
            }

            this.ImportAuthenticationData();
            await this.StartFunctionAsync();
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
            if (this.container.Function != null)
            {
                return;
            }

            await this.container.AllocateFunctionAsync(this.Auth);
            await this.container.GetNewestItemsAsync();
            await this.container.Function.StartAsync();
        }

        private async Task StopFunctionAsync()
        {
            if (this.container.Function == null)
            {
                return;
            }

            await this.container.ReleaseFunctionAsync();
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

        /// <summary>
        /// 削除する
        /// </summary>
        public RelayCommand<MastodonStatus> DeleteCommand
        {
            get
            {
                return this._deleteCommand = this._deleteCommand ?? new RelayCommand<MastodonStatus>((status) =>
                {
                    status.DeleteAsync(this.client);
                });
            }
        }
        private RelayCommand<MastodonStatus> _deleteCommand;

        /// <summary>
        /// トゥートの詳細を表示する
        /// </summary>
        public RelayCommand<MastodonStatus> ShowStatusDetailCommand
        {
            get
            {
                return this._showStatusDetailCommand = this._showStatusDetailCommand ?? new RelayCommand<MastodonStatus>((status) =>
                {
                    var container = new StatusDetailFunctionContainer(status.Id);
                    this.PushNewFrame(container);
                });
            }
        }
        private RelayCommand<MastodonStatus> _showStatusDetailCommand;

        /// <summary>
        /// フレーム内の画面遷移で、元のページに戻る
        /// </summary>
        public RelayCommand BackFrameCommand
        {
            get
            {
                return this._backFrameCommand = this._backFrameCommand ?? new RelayCommand(() =>
                {
                    this.FrameStack.Pop();
                });
            }
        }
        private RelayCommand _backFrameCommand;

        #endregion
    }
}
