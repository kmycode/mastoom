using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Mastoom.UWP.Converters
{
    class Boolean2NegativeVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool val && targetType == typeof(Visibility))
            {
                return !val ? Visibility.Visible : Visibility.Collapsed;
            }
            throw new NotSupportedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
