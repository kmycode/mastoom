using Mastonet;
using Mastonet.Entities;
using Mastoom.Shared.Models.Common;
using Mastoom.Shared.Models.Mastodon.Account;
using Mastoom.Shared.Models.Mastodon.Connection.Function;
using Mastoom.Shared.Models.Mastodon.Notification;
using Mastoom.Shared.Models.Mastodon.Status;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mastoom.Shared.Models.Mastodon.Connection
{
    /// <summary>
    /// マストドンへのひとつの認証
    /// </summary>
    public class MastodonAuthentication
    {
        #region 変数

        /// <summary>
        /// マストドンへの接続情報
        /// </summary>
        private AppRegistration appRegistration;

        /// <summary>
        /// 認証する前のクライアント
        /// </summary>
        private AuthenticationClient preClient;

        #endregion

        #region プロパティ

        /// <summary>
        /// 認証が完了したか
        /// </summary>
        public bool HasAuthenticated { get; private set; }

        /// <summary>
        /// アクセストークン
        /// </summary>
        public string AccessToken { get; private set; }

        /// <summary>
        /// OAuthブラウザを操作するヘルパ
        /// </summary>
        public OAuthModel OAuthHelper { get; } = new OAuthModel();

        /// <summary>
        /// マストドンへの接続を行うクライアント。このクラス内の処理で生成
        /// </summary>
        public MastodonClient Client { get; private set; }

        /// <summary>
        /// インスタンスのURI
        /// </summary>
        public string InstanceUri { get; }

        /// <summary>
        /// ストリーミングのURI
        /// </summary>
        public string StreamingUri { get; private set; }

        /// <summary>
        /// この認証情報で得られるユーザ
        /// </summary>
        public MastodonAccount CurrentUser { get; private set; }

        /// <summary>
        /// 公開タイムラインへアクセスする機能のカウンタ
        /// </summary>
        public ConnectionFunctionCounter<MastodonStatus> PublicStreamingFunctionCounter { get; private set; }

        /// <summary>
        /// ホームタイムラインへアクセスする機能のカウンタ
        /// </summary>
        public ConnectionFunctionCounter<MastodonStatus> HomeStreamingFunctionCounter { get; private set; }

        /// <summary>
        /// 通知へアクセスする機能のカウンタ
        /// </summary>
        public ConnectionFunctionCounterBase<MastodonNotification> NotificationStreamingFunctionCounter { get; private set; }

        #endregion

        #region メソッド

        public MastodonAuthentication(string instanceUri, string accessToken = null)
        {
            this.InstanceUri = instanceUri;
            this.AccessToken = accessToken;
            this.appRegistration = this.GetAppRegistration();

            if (string.IsNullOrEmpty(accessToken))
            {
                // ヘルパがビヘイビアにアタッチされた時
                this.OAuthHelper.Attached += (sender, e) =>
                {
                    // すぐさまログインを開始
                    this.StartOAuthLogin();
                };
            }
            else
            {
                Task.Run(async () =>
                {
                    try
                    {
                        await this.CreateClientAsync();
                        this.HasAuthenticated = true;
                    }
                    catch
                    {
                        this.HasAuthenticated = false;
                    }
                });
            }

            // ブラウザのURIが変わった時
            this.OAuthHelper.UriNavigated += async (sender, e) =>
            {
                try
                {
                    if (e.Uri.Contains("/oauth/authorize/"))
                    {
                        var paths = e.Uri.Split('/');
                        this.AccessToken = paths.Last();

                        await this.CreateClientAsync();

                        this.OAuthHelper.Hide();
                        this.HasAuthenticated = true;
                        this.Completed?.Invoke(this, new EventArgs());
                    }
                }
                catch
                {
                    this.HasAuthenticated = false;
                    this.OnError?.Invoke(this, new EventArgs());
                }
            };

        }

        private AppRegistration GetAppRegistration()
        {
            var data = InstanceAccessProvider.GetWithInstanceUri(this.InstanceUri);

            this.StreamingUri = data.StreamingInstance;
            //var appRegistration = await MastodonClient.CreateApp(this.instanceUrl, ApplicationName, Scope.Follow | Scope.Read | Scope.Write, "http://kmycode.net/");
            appRegistration = new AppRegistration
            {
                Instance = this.InstanceUri,
                ClientId = data.ClientId,
                ClientSecret = data.ClientSecret,
                Scope = Scope.Follow | Scope.Read | Scope.Write,
            };

            return appRegistration;
        }

        private async Task CreateClientAsync()
        {
            var auth = await this.preClient.ConnectWithCode(this.AccessToken);
            this.Client = new MastodonClient(this.appRegistration, auth);
            this.CurrentUser = new MastodonAccount(await this.Client.GetCurrentUser());

            this.PublicStreamingFunctionCounter = new ConnectionFunctionCounter<MastodonStatus>(new PublicTimelineFunction
            {
                Client = this.Client,
                StreamingInstanceUri = this.StreamingUri,
            });

            var homeFunc = new HomeTimelineFunction
            {
                Client = this.Client,
                StreamingInstanceUri = this.StreamingUri,
            };
            this.HomeStreamingFunctionCounter = new ConnectionFunctionCounter<MastodonStatus>(homeFunc);

            this.NotificationStreamingFunctionCounter = new ConnectionFunctionCompositionCounter<MastodonNotification, MastodonStatus>(
                new NotificationTimelineFunction(this.Client, homeFunc), this.HomeStreamingFunctionCounter
                );
        }

        public void StartOAuthLogin()
        {
            this.OAuthHelper.Show();
            this.preClient = new AuthenticationClient(this.appRegistration);
            this.OAuthHelper.NavigateRequest(this.preClient.OAuthUrl());
        }

        #endregion

        #region イベント

        /// <summary>
        /// 認証が完了した時に発行
        /// </summary>
        public EventHandler Completed;

        /// <summary>
        /// エラー発生した時に発行
        /// </summary>
        public EventHandler OnError;

        #endregion
    }
}
