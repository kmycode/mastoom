using Mastoom.Shared.Models.Mastodon.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mastoom.Shared.Models.Mastodon.Notification
{
    /// <summary>
    /// 通知のコレクション。同一IDのオブジェクトだけでなく、
    /// 「同じステータスをふぁぼした」「同じステータスをBTした」というデータも同時に存在しないことが保証される。
    /// なお、同じステータスのふぁぼとBTは別々のデータとして一緒に入れられる
    /// </summary>
    public class MastodonNotificationCollection : MastodonObjectCollection<MastodonNotification>
    {
        protected override void CopyObject(MastodonNotification from, MastodonNotification to)
        {
        }

        public override void Add(MastodonNotification obj)
        {
            if (!this.Filter(obj))
            {
                return;
            }

            var existing = this.FindNotification(obj.Status?.Id, obj.Type);
            if (existing != null)
            {
                this.CopyObject(obj, existing);
            }
            else
            {
                this.ForceAdd(obj);
            }
        }

        /// <summary>
        /// 趣旨の重複する通知が他にないか調べる
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private MastodonNotification FindNotification(int? id, NotificationType type)
        {
            return this.SingleOrDefault(noti =>
            {
                if (id != null && noti.Status != null)
                    return noti.Status.Id == id && noti.Type == type;
                else
                    return false;
            });
        }
    }
}
