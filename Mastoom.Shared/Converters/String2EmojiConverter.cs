using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
#if UWP
using Windows.UI.Xaml.Data;
#else
using Xamarin.Forms;
#endif

namespace Mastoom.Shared.Converters
{
	public class String2EmojiConverter : IValueConverter
	{
		private static readonly Dictionary<String, EmojiSharp.Emoji> dic = EmojiSharp.Emoji.All;

#if UWP
		public object Convert(object value, Type targetType, object parameter, string language)
#else
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo info)
#endif
		{
			string text = value as string;
			if (text != null && targetType == typeof(string))
			{
				if (string.IsNullOrWhiteSpace(text) || !text.Contains(":"))
				{
					return text;
				}

				string strRegex = @":(.[a-zA-Z0-9_]+):";
				Regex myRegex = new Regex(strRegex, RegexOptions.None);
				string strTargetString = text;

				var succeedMatches = myRegex.Matches(strTargetString).OfType<Match>().Where(mt => mt.Success);
				foreach (Match myMatch in succeedMatches)
				{
					foreach (Group group in myMatch.Groups)
					{
						var key = group.Value.Substring(1, group.Value.Length - 2);

						if (dic.ContainsKey(key))
						{
							var charCodes = dic[key].Unified.Split('-');
							var newString = "";
							foreach (var code in charCodes)
							{
								newString += ((char)(System.Convert.ToInt32(code, 16))).ToString();
							}
							text = text.Replace(group.Value, newString);
						}
					}
				}

				return text;
			}
			throw new NotSupportedException();
		}

#if UWP
		public object ConvertBack(object value, Type targetType, object parameter, string language)
#else
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo info)
#endif
		{
			throw new NotImplementedException();
		}
	}
}
