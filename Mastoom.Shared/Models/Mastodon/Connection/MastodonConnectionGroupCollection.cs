using Mastoom.Shared.Models.Mastodon.Connection.Function.Container;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace Mastoom.Shared.Models.Mastodon.Connection
{
    public class MastodonConnectionGroupCollection : ObservableCollection<MastodonConnectionGroup>
    {
#if DEBUG
        public void AddTestConnection()
        {
            this.Add(new MastodonConnectionGroup
            {
                Name = "Pawoo",
            });
            this[0].TryAdd(new MastodonConnection("pawoo.net", new HomeTimelineFunctionContainer())
            {
                Name = "ホーム",
            });
            this[0].TryAdd(new MastodonConnection("pawoo.net", new NotificationFunctionContainer())
            {
                Name = "通知",
            });
            this[0].TryAdd(new MastodonConnection("pawoo.net", new LocalTimelineFunctionContainer())
            {
                Name = "ローカルタイムライン",
            });
            this[0].TryAdd(new MastodonConnection("pawoo.net", new PublicTimelineFunctionContainer())
            {
                Name = "公開タイムライン",
            });

            this.Add(new MastodonConnectionGroup
            {
                Name = "Mstdn",
            });
            this[1].TryAdd(new MastodonConnection("mstdn.jp", new HomeTimelineFunctionContainer())
            {
                Name = "ホーム",
            });
            this[1].TryAdd(new MastodonConnection("mstdn.jp", new NotificationFunctionContainer())
            {
                Name = "通知",
            });
            this[1].TryAdd(new MastodonConnection("mstdn.jp", new LocalTimelineFunctionContainer())
            {
                Name = "ローカルタイムライン",
            });
            this[1].TryAdd(new MastodonConnection("mstdn.jp", new PublicTimelineFunctionContainer())
            {
                Name = "公開タイムライン",
            });
        }
#endif
    }
}
