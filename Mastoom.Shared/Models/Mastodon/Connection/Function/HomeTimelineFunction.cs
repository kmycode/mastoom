using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Mastonet;
using Mastoom.Shared.Models.Mastodon.Notification;

namespace Mastoom.Shared.Models.Mastodon.Connection.Function
{
    /// <summary>
    /// ホームタイムラインのファンクション
    /// </summary>
    class HomeTimelineFunction : TimelineFunctionBase
    {
        protected override TimelineStreaming GetTimelineStreaming(string streamInstanceUri = null)
        {
            TimelineStreaming streaming;

            if (string.IsNullOrEmpty(streamInstanceUri))
            {
                streaming = this.Client.GetUserStreaming();
            }
            else
            {
                streaming = this.Client.GetUserStreaming(this.StreamingInstanceUri);
            }

            streaming.OnNotification += this.Streaming_OnNotification;
            return streaming;
        }

        public override async Task StopAsync()
        {
            this.Streaming.OnNotification -= this.Streaming_OnNotification;
            await base.StopAsync();
        }

        private void Streaming_OnNotification(object sender, StreamNotificationEventArgs e)
        {
            this.Notificated?.Invoke(this, new ObjectFunctionUpdateEventArgs<MastodonNotification>
            {
                Object = new MastodonNotification(e.Notification),
            });
        }

        /// <summary>
        /// 通知が来た時にくるイベント
        /// </summary>
        public event EventHandler<ObjectFunctionUpdateEventArgs<MastodonNotification>> Notificated;
    }
}
