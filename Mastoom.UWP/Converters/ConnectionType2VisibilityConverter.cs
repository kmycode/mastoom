using Mastoom.Shared.Models.Mastodon.Connection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Mastoom.UWP.Converters
{
    class ConnectionType2VisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is ConnectionType && targetType == typeof(Visibility))
            {
                switch ((ConnectionType)value)
                {
                    case ConnectionType.HomeTimeline:
                    case ConnectionType.LocalTimeline:
                    case ConnectionType.PublicTimeline:
                        return parameter.ToString() == "Status";
                    case ConnectionType.Notification:
                        return parameter.ToString() == "Notification";
                }
            }
            throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
