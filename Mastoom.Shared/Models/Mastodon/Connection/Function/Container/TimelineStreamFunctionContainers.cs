using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Mastoom.Shared.Models.Mastodon.Generic;
using Mastoom.Shared.Models.Mastodon.Status;
using Mastoom.Shared.Common;

namespace Mastoom.Shared.Models.Mastodon.Connection.Function.Container
{
    abstract class TimelineFunctionContainerBase : FunctionContainerBase<MastodonStatus>
    {
        public TimelineFunctionContainerBase(MastodonObjectCollection<MastodonStatus> collection, ConnectionType type) : base(collection, type)
        {
        }

        protected override void OnFunctionUpdated(object sender, ObjectFunctionEventArgs<MastodonStatus> e)
        {
            base.OnFunctionUpdated(sender, e);

            // 自分の発言にマークを付ける
            if (e.Object.Account.Id == this.Auth.CurrentUser.Id)
            {
                e.Object.IsMyStatus = true;
            }
        }
    }

    class PublicTimelineFunctionContainer : TimelineFunctionContainerBase
    {
        public PublicTimelineFunctionContainer() : this(ConnectionType.PublicTimeline) { }

        protected PublicTimelineFunctionContainer(ConnectionType type) : base(new MastodonStatusCollection(), type) { }

        protected override ConnectionFunctionCounterBase<MastodonStatus> GetFunctionCounter(MastodonAuthentication auth)
            => auth.PublicStreamingFunctionCounter;
    }

    class LocalTimelineFunctionContainer : PublicTimelineFunctionContainer
    {
        public LocalTimelineFunctionContainer() : base(ConnectionType.LocalTimeline) { }

        protected override void OnFunctionAllocated()
        {
            base.OnFunctionAllocated();
            this._objects.Filter = (status) => status.Account.IsLocal;
        }
    }

    class HomeTimelineFunctionContainer : TimelineFunctionContainerBase
    {
        public HomeTimelineFunctionContainer() : base(new MastodonStatusCollection(), ConnectionType.HomeTimeline) { }

        protected override ConnectionFunctionCounterBase<MastodonStatus> GetFunctionCounter(MastodonAuthentication auth)
            => auth.HomeStreamingFunctionCounter;
    }
}
