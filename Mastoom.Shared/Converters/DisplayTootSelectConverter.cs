using Mastoom.Shared.Models.Mastodon.Status;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Data;

namespace Mastoom.Shared.Converters
{
    public class DisplayTootSelectConverter : SharedConverterBase
    {
        public override object Convert(object value, Type targetType, object parameter, string language)
        {
            var status = value as MastodonStatus;
            if (status != null)
            {
                return status.IsBoost ? status.Boost : status;
            }
            throw new NotImplementedException();
        }

        public override object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
