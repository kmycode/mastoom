using Mastoom.Shared.Models.Mastodon.Account;
using Mastoom.Shared.Models.Mastodon.Status;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mastoom.Shared.Models.Mastodon.Notification
{
    public class MastodonNotification
    {
        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// 通知の理由となった行動を起こしたアカウント
        /// </summary>
        public MastodonAccount Account { get; }

        /// <summary>
        /// 通知の理由となった行動の対象トゥート
        /// </summary>
        public MastodonStatus Status { get; }

        /// <summary>
        /// 通知の種類
        /// </summary>
        public NotificationType Type { get; }

        public MastodonNotification(Mastonet.Entities.Notification notification)
        {
            this.Id = notification.Id;
            this.Account = new MastodonAccount(notification.Account);
            this.Status = new MastodonStatus(notification.Status);

            switch (notification.Type)
            {
                case "mention":
                    this.Type = NotificationType.Mention;
                    break;
                case "reblog":
                    this.Type = NotificationType.Boost;
                    break;
                case "favourite":
                    this.Type = NotificationType.Favorite;
                    break;
                case "follow":
                    this.Type = NotificationType.Follow;
                    break;
                default:
                    this.Type = NotificationType.Unknown;
                    break;
            }
        }
    }
}
