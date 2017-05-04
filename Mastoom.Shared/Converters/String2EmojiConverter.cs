using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Mastoom.Shared.Converters
{
    public class String2EmojiConverter : SharedConverterBase
    {
		private static readonly Dictionary<String, EmojiSharp.Emoji> dic = EmojiSharp.Emoji.All;

		public override object Convert(object value, Type targetType, object parameter, string language)
		{
			string text = value as string;
			if (text != null && targetType == typeof(string))
			{
				if (string.IsNullOrWhiteSpace(text) || !text.Contains(":"))
				{
					return text;
				}

                string strRegex = @":([a-zA-Z0-9_]+):";
                Regex myRegex = new Regex(strRegex, RegexOptions.None);
                string strTargetString = text;

                var succeedMatchGroups = myRegex.Matches(strTargetString)
                                                .OfType<Match>().Where(mt => mt.Success)
                                                .SelectMany(mt => mt.Groups.OfType<Group>());
                foreach (Group group in succeedMatchGroups)
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

                return text;
			}
			throw new NotSupportedException();
		}

		public override object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new NotImplementedException();
		}
	}
}
