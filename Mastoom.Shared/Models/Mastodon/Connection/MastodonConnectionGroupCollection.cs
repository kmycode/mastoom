using Mastoom.Shared.Models.Mastodon.Connection.Function.Container;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using Mastoom.Shared.Repositories;

namespace Mastoom.Shared.Models.Mastodon.Connection
{
    public class MastodonConnectionGroupCollection : ObservableCollection<MastodonConnectionGroup>
    {
        private readonly OAuthAccessTokenRepository tokenRepo = new OAuthAccessTokenRepository();
#if DEBUG
        public void AddTestConnection()
        {
            this.Add(new MastodonConnectionGroup
            {
                Name = "Pawoo",
            });
            this[0].TryAdd(new MastodonConnection("pawoo.net", new HomeTimelineFunctionContainer(), tokenRepo)
            {
                Name = "ホーム",
            });
            this[0].TryAdd(new MastodonConnection("pawoo.net", new NotificationFunctionContainer(), tokenRepo)
            {
                Name = "通知",
            });
            this[0].TryAdd(new MastodonConnection("pawoo.net", new LocalTimelineFunctionContainer(), tokenRepo)
            {
                Name = "ローカルタイムライン",
            });
            this[0].TryAdd(new MastodonConnection("pawoo.net", new PublicTimelineFunctionContainer(), tokenRepo)
            {
                Name = "公開タイムライン",
            });

            this.Add(new MastodonConnectionGroup
            {
                Name = "Mstdn",
            });
            this[1].TryAdd(new MastodonConnection("mstdn.jp", new HomeTimelineFunctionContainer(), tokenRepo)
            {
                Name = "ホーム",
            });
            this[1].TryAdd(new MastodonConnection("mstdn.jp", new NotificationFunctionContainer(), tokenRepo)
            {
                Name = "通知",
            });
            this[1].TryAdd(new MastodonConnection("mstdn.jp", new LocalTimelineFunctionContainer(), tokenRepo)
            {
                Name = "ローカルタイムライン",
            });
            this[1].TryAdd(new MastodonConnection("mstdn.jp", new PublicTimelineFunctionContainer(), tokenRepo)
            {
                Name = "公開タイムライン",
            });
        }
#endif
    }
}
