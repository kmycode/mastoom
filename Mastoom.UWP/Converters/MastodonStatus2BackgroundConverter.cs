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

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var status = value as MastodonStatus;
            if (status != null && targetType == typeof(Brush))
            {
                if (status.IsBoost)
                {
                    return RebootBrush;
                }
                return null;
            }
            throw new NotSupportedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
