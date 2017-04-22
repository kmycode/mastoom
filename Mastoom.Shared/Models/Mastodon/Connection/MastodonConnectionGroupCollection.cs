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
            this[0].TryAdd(new MastodonConnection("pawoo.net", ConnectionType.PublicTimeline)
            {
                Name = "公開タイムライン",
            });

            this.Add(new MastodonConnectionGroup
            {
                Name = "Mstdn",
            });
            this[1].TryAdd(new MastodonConnection("mstdn.jp", ConnectionType.HomeTimeline)
            {
                Name = "ホーム",
            });
            this[1].TryAdd(new MastodonConnection("mstdn.jp", ConnectionType.LocalTimeline)
            {
                Name = "ローカルタイムライン",
            });
            this[1].TryAdd(new MastodonConnection("mstdn.jp", ConnectionType.PublicTimeline)
            {
                Name = "公開タイムライン",
            });
        }
#endif
    }
}
