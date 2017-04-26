using Mastoom.Shared.Models.Mastodon.Notification;
using Mastoom.Shared.Models.Mastodon.Status;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace Mastoom.UWP.Converters
{
    class MastodonStatus2BackgroundConverter : IValueConverter
    {
        private static readonly Brush RebootBrush = new SolidColorBrush(new Color { R = 0x00, G = 0x80, B = 0x00, A = 0x20, });
        private static readonly Brush FavoriteBrush = new SolidColorBrush(new Color { R = 0xa0, G = 0xa0, B = 0x00, A = 0x20, });

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (targetType == typeof(Brush))
            {
                var status = value as MastodonStatus;
                if (status != null)
                {
                    if (status.IsBoost)
                    {
                        return RebootBrush;
                    }
                    return null;
                }
                var notification = value as MastodonNotification;
                if (notification != null)
                {
                    if (notification.Type == NotificationType.Boost)
                    {
                        return RebootBrush;
                    }
                    else if (notification.Type == NotificationType.Favorite)
                    {
                        return FavoriteBrush;
                    }
                    return null;
                }
            }
            throw new NotSupportedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
