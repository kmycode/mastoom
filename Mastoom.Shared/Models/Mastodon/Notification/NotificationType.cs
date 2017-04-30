using System;
using System.Collections.Generic;
using System.Text;

namespace Mastoom.Shared.Models.Mastodon.Notification
{
    /// <summary>
    /// 通知の種別
    /// </summary>
    public enum NotificationType
    {
        /// <summary>
        /// 不明
        /// </summary>
        Unknown,

        /// <summary>
        /// メンション（リプ）
        /// </summary>
        Mention,

        /// <summary>
        /// ブースト（仕様上はreblogとなってる）
        /// </summary>
        Boost,

        /// <summary>
        /// ふぁぼ
        /// </summary>
        Favorite,

        /// <summary>
        /// フォロー
        /// </summary>
        Follow,
    }
}
