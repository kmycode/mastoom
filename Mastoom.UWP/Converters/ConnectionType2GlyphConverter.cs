using Mastoom.Shared.Models.Mastodon.Connection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Mastoom.UWP.Converters
{
    class ConnectionType2GlyphConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is ConnectionType && targetType == typeof(string))
            {
                var type = (ConnectionType)value;

                switch (type)
                {
                    case ConnectionType.PublicTimeline:
                        return "\uE128";
                    case ConnectionType.LocalTimeline:
                        return "\uE7C1";
                    case ConnectionType.HomeTimeline:
                        return "\uE80F";
                    case ConnectionType.Notification:
                        return "\uED0D";
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
