using Mastoom.Shared.Models.Mastodon.Notification;
using Mastoom.Shared.Models.Mastodon.Status;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Mastoom.UWP.Views.Helpers
{
    class TimelineDataTemplateSelector : DataTemplateSelector
    {
        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (item is MastodonStatus)
            {
                return (DataTemplate)Application.Current.Resources["TimelineStatusTemplate"];
            }
            else if (item is MastodonNotification)
            {
                var n = (MastodonNotification)item;
                if (n.Type == NotificationType.Boost || n.Type == NotificationType.Favorite || n.Type == NotificationType.Mention)
                {
                    return (DataTemplate)Application.Current.Resources["NotificationStatusTemplate"];
                }
                else
                {
                    return (DataTemplate)Application.Current.Resources["NotificationAccountTemplate"];
                }
            }
            return null;
        }
    }
}
