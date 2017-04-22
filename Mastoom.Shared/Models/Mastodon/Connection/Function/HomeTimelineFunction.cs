using System;
using System.Collections.Generic;
using System.Text;
using Mastonet;

namespace Mastoom.Shared.Models.Mastodon.Connection.Function
{
    /// <summary>
    /// ホームタイムラインのファンクション
    /// </summary>
    class HomeTimelineFunction : TimelineFunctionBase
    {
        protected override TimelineStreaming GetTimelineStreaming(string streamInstanceUri = null)
        {
            if (string.IsNullOrEmpty(streamInstanceUri))
            {
                return this.Client.GetUserStreaming();
            }
            else
            {
                return this.Client.GetUserStreaming(this.StreamingInstanceUri);
            }
        }
    }
}
