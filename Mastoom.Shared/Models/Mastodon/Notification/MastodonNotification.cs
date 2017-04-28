using Mastoom.Shared.Models.Mastodon.Account;
using Mastoom.Shared.Models.Mastodon.Generic;
using Mastoom.Shared.Models.Mastodon.Status;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Mastoom.Shared.Models.Mastodon.Notification
{
    public class MastodonNotification : MastodonObject
    {
        /// <summary>
        /// 通知の理由となった行動を起こしたアカウント
        /// </summary>
        public IReadOnlyCollection<MastodonAccount> Accounts => this._accounts;
        private Collection<MastodonAccount> _accounts = new Collection<MastodonAccount>();

        /// <summary>
        /// 通知の理由となった行動の対象トゥート
        /// </summary>
        public MastodonStatus Status { get; }

        /// <summary>
        /// 通知の種類
        /// </summary>
        public NotificationType Type { get; }

        public MastodonNotification(Mastonet.Entities.Notification notification) : base(notification.Id)
        {
            if (notification.Account != null)
            {
                this._accounts.Add(notification.Account.ToMastodonAccount());
            }
            if (notification.Status != null)
            {
                this.Status = notification.Status.ToMastodonStatus();
            }

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

        public void MergeTo(MastodonNotification to)
        {
            if (to.Status.Id != this.Status.Id)
            {
                throw new InvalidOperationException("コピーしようとするStatusのIDが違います");
            }
            foreach (var account in this.Accounts)
            {
                to._accounts.Add(account);
            }
        }
    }
}
