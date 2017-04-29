using Mastoom.Shared.Models.Mastodon.Status;
using System;

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
