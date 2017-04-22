using Mastonet;
using Mastoom.Shared.Models.Mastodon.Status;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mastoom.Shared.Models.Mastodon.Connection.Function
{
    /// <summary>
    /// タイムラインストリーミングを利用するConnectionFunctionの基底クラス
    /// </summary>
    abstract class TimelineFunctionBase : IConnectionFunction<MastodonStatus>
    {
        public MastodonClient Client { protected get; set; }
        public string StreamingInstanceUri { get; set; }

        private TimelineStreaming streaming;
        private bool isStreaming;

        private const int StreamingTimeOut = 10;    // 一定時間ストリーミングの更新がない時に接続し直す（バグ？）
        private Timer timer;

        public event EventHandler<ObjectFunctionUpdateEventArgs<MastodonStatus>> Updated;
        public event EventHandler<ObjectFunctionEventArgs<MastodonStatus>> Deleted;
        public event EventHandler<ObjectFunctionErrorEventArgs> Errored;

        public TimelineFunctionBase()
        {
            // ストリーミングの更新がタイムアウトした時の処理
            this.timer = new Timer(async (sender) =>
            {
                await this.StopAsync();
                await this.StartAsync();
            }, null, Timeout.Infinite, Timeout.Infinite);
        }

        public async Task GetNewerAsync()
        {
        }

        public async Task GetPrevAsync(int maxId)
        {
        }

        public async Task StartAsync()
        {
            await this.StartAsync(1);
        }

        protected abstract TimelineStreaming GetTimelineStreaming(string streamInstanceUri = null);

        private async Task StartAsync(int tryCount, Exception lastException = null)
        {
            if (this.isStreaming)
            {
                return;
            }

            // 試行回数
            if (tryCount > 3)
            {
                this.Errored?.Invoke(this, new ObjectFunctionErrorEventArgs
                {
                    Exception = lastException,
                    Type = ObjectFunctionErrorType.StartError,
                });
                return;
            }

            try
            {
                if (this.streaming == null)
                {
                    this.streaming = this.GetTimelineStreaming(this.StreamingInstanceUri);

                    this.streaming.OnUpdate += this.Streaming_OnUpdate;
                    this.streaming.OnDelete += this.Streaming_OnDelete;
                }

                this.timer.Change(1000 * StreamingTimeOut, Timeout.Infinite);
                this.streaming.Start();

                this.isStreaming = true;
            }
            catch (Exception e)
            {
                await this.StopAsync();
                await this.StartAsync(tryCount + 1, e);
            }
        }

        public async Task StopAsync()
        {
            if (this.isStreaming)
            {
                try
                {
                    this.streaming.Stop();
                }
                catch { }
                this.streaming.OnUpdate -= this.Streaming_OnUpdate;
                this.streaming.OnDelete -= this.Streaming_OnDelete;
                this.streaming = null;
                this.timer.Change(Timeout.Infinite, Timeout.Infinite);

                this.isStreaming = false;
            }
        }

        private void Streaming_OnUpdate(object sender, StreamUpdateEventArgs e)
        {
            this.timer.Change(1000 * StreamingTimeOut, Timeout.Infinite);
            this.Updated?.Invoke(this, new ObjectFunctionUpdateEventArgs<MastodonStatus>
            {
                Object = new MastodonStatus(e.Status),
            });
        }

        private void Streaming_OnDelete(object sender, StreamDeleteEventArgs e)
        {
            this.Deleted?.Invoke(this, new ObjectFunctionEventArgs<MastodonStatus>
            {
                Object = new MastodonStatus(e.StatusId, null),
            });
        }
    }
}
