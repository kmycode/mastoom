using Mastonet;
using Mastoom.Shared.Models.Mastodon.Notification;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

namespace Mastoom.Shared.Models.Mastodon.Connection.Function
{
    /// <summary>
    /// 通知を知らせるファンクション。
    /// HomeTimelineのFunctionを乗っ取る
    /// </summary>
    class NotificationTimelineFunction : IConnectionFunction<MastodonNotification>
    {
        private readonly HomeTimelineFunction homeFunction;
        private readonly MastodonClient client;

        public event EventHandler<ObjectFunctionUpdateEventArgs<MastodonNotification>> Updated;
        public event EventHandler<ObjectFunctionEventArgs<MastodonNotification>> Deleted;
        public event EventHandler<ObjectFunctionErrorEventArgs> Errored;

        public NotificationTimelineFunction(MastodonClient client, HomeTimelineFunction homeFunc)
        {
            this.client = client;

            this.homeFunction = homeFunc;
            this.homeFunction.Notificated += (sender, e) =>
            {
                this.Updated?.Invoke(this, new ObjectFunctionUpdateEventArgs<MastodonNotification>
                {
                    Object = e.Object,
                });
            };
        }

        public async Task<IEnumerable<MastodonNotification>> GetNewestAsync()
        {
            var nativeNotifications = await this.client.GetNotifications();
            var notifications = new Collection<MastodonNotification>();
            foreach (var n in nativeNotifications)
            {
                notifications.Add(new MastodonNotification(n));
            }
            return notifications;
        }

        public async Task<IEnumerable<MastodonNotification>> GetPrevAsync(int maxId)
        {
            return null;
        }

        public async Task StartAsync()
        {
            await this.homeFunction.StartAsync();
        }

        public async Task StopAsync()
        {
            await this.homeFunction.StopAsync();
        }
    }
}
