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
    /// 公開タイムラインのファンクション
    /// </summary>
    class PublicTimelineFunction : TimelineFunctionBase
    {
        protected override TimelineStreaming GetTimelineStreaming(string streamInstanceUri = null)
        {
            if (string.IsNullOrEmpty(streamInstanceUri))
            {
                return this.Client.GetPublicStreaming();
            }
            else
            {
                return this.Client.GetPublicStreaming(this.StreamingInstanceUri);
            }
        }
    }
}
