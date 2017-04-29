﻿using System;
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
        public static ReadOnlyObservableCollection<MastodonAuthentication> Authes { get; }
        private static readonly ObservableCollection<MastodonAuthentication> _authes = new ObservableCollection<MastodonAuthentication>();

        static MastodonAuthenticationHouse()
        {
            Authes = new ReadOnlyObservableCollection<MastodonAuthentication>(_authes);
        }

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
            if (auth != null)
            {
                return auth;
            }
            else
            {
                var newAuth = new MastodonAuthentication(instanceUri);
                _authes.Add(newAuth);
                return newAuth;
            }
        }
    }
}
