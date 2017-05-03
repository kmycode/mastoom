using System;
using System.Globalization;
using Xamarin.Forms;

namespace Mastoom.Converters
{
	public class Time2ElapseConverter : IValueConverter
	{
		object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var time = value as DateTime?;
			if (!time.HasValue)
			{
				return "--";
			}

			var localTime = time.Value.ToLocalTime();

			// TODO ちゃんと考えないとだめだわ
			// 時刻が変わったらリストの全アイテム更新しなきゃなのでコスト高い
			//var diff = DateTime.Now - time.Value;
			//
			//if (diff.TotalSeconds < 30) // 30秒未満なら "今"
			//{
			//	return "now";
			//}
			//else if (diff.TotalSeconds < 60) // 60秒未満なら "n秒"
			//{
			//	return $"{diff.TotalSeconds}s";
			//}
			//else if (diff.TotalMinutes < 60) // 60分未満なら "n分"
			//{
			//	return $"{diff.Minutes}m";
			//}
			//else if (diff.TotalHours < 24) // 24時間未満なら "n時間"
			//{
			//	return $"{diff.Hours}h";
			//}
			//else if (diff.TotalDays < 365) // 365日未満なら "n日"
			//{
			//	return $"{diff.Days}d";
			//}
			//else
			//{
			//	int year = (int)diff.TotalDays / 365;
			//	return $"{year}y";
			//}

			return $"{localTime:M/d h:m}";

		}

		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
