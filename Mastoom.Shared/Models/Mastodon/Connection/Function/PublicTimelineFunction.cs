using Mastonet;
using Mastoom.Shared.Models.Mastodon.Status;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public override async Task<IEnumerable<MastodonStatus>> GetNewestAsync()
        {
            var nativeStatuses = await this.Client.GetPublicTimeline();
            var statuses = new Collection<MastodonStatus>();
            foreach (var s in nativeStatuses)
            {
                statuses.Add(new MastodonStatus(s));
            }
            return statuses;
        }

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
