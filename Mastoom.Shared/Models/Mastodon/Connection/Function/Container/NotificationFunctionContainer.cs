using Mastoom.Shared.Models.Mastodon.Notification;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mastoom.Shared.Models.Mastodon.Connection.Function.Container
{
    class NotificationFunctionContainer : FunctionContainerBase<MastodonNotification>
    {
        public NotificationFunctionContainer() : base(new MastodonNotificationCollection(), ConnectionType.Notification) { }

        protected override ConnectionFunctionCounterBase<MastodonNotification> GetFunctionCounter(MastodonAuthentication auth)
            => auth.NotificationStreamingFunctionCounter;
    }
}
