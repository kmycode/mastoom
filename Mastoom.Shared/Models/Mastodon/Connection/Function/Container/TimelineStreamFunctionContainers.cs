using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Mastoom.Shared.Models.Mastodon.Generic;
using Mastoom.Shared.Models.Mastodon.Status;
using Mastoom.Shared.Common;

namespace Mastoom.Shared.Models.Mastodon.Connection.Function.Container
{
    class PublicTimelineFunctionContainer : FunctionContainerBase<MastodonStatus>
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

    class HomeTimelineFunctionContainer : FunctionContainerBase<MastodonStatus>
    {
        public HomeTimelineFunctionContainer() : base(new MastodonStatusCollection(), ConnectionType.HomeTimeline) { }

        protected override ConnectionFunctionCounterBase<MastodonStatus> GetFunctionCounter(MastodonAuthentication auth)
            => auth.HomeStreamingFunctionCounter;
    }
}
