using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
#if WINDOWS_UWP
using Windows.UI.Xaml.Data;
#else
using Xamarin.Forms;
#endif

namespace Mastoom.Shared.Converters
{
    public abstract class SharedConverterBase : IValueConverter
    {
        public abstract object Convert(object value, Type targetType, object parameter, string language);
        public abstract object ConvertBack(object value, Type targetType, object parameter, string language);

#if WINDOWS_UWP
#else
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return this.Convert(value, targetType, parameter, culture?.Name);
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return this.ConvertBack(value, targetType, parameter, culture?.Name);
        }
#endif
    }
}
