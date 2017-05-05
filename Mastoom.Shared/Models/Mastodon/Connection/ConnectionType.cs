using System;
using System.Collections.Generic;
using System.Text;

namespace Mastoom.Shared.Models.Mastodon.Connection
{
    /// <summary>
    /// 接続のタイプ
    /// </summary>
    public enum ConnectionType
    {
        /// <summary>
        /// 公開タイムライン
        /// </summary>
        PublicTimeline,

        /// <summary>
        /// ローカルタイムライン
        /// </summary>
        LocalTimeline,

        /// <summary>
        /// ホームタイムライン
        /// </summary>
        HomeTimeline,

        /// <summary>
        /// 通知
        /// </summary>
        Notification,

        /// <summary>
        /// トゥートの詳細
        /// </summary>
        StatusDetail,
    }
}
