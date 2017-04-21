using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Mastoom.Shared.Models.Mastodon.Connection
{
    /// <summary>
    /// マストドンの認証情報を保管する
    /// </summary>
    static class MastodonAuthenticationHouse
    {
        /// <summary>
        /// すでに保管された認証
        /// </summary>
        public static Collection<MastodonAuthentication> Authes { get; } = new Collection<MastodonAuthentication>();

        /// <summary>
        /// マストドンの認証情報を取得。
        /// 取得した情報にはHasAuthenticatedがfalse（未認証状態）の場合があるので
        /// その場合は呼び出し側が認証すること
        /// </summary>
        /// <param name="instanceUri">インスタンスURI</param>
        /// <returns>認証情報</returns>
        public static MastodonAuthentication Get(string instanceUri)
        {
            var auth = Authes.SingleOrDefault(a => a.InstanceUri == instanceUri);
            return auth ?? new MastodonAuthentication(instanceUri);
        }
    }
}
