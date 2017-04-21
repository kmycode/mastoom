using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace Mastoom.Views
{
	/// <summary>
	/// 他のインスタンス"群"のタイムラインを表示する View。クラス名微妙。
	/// いわゆる「オレオレ連合タイムライン」。
	/// </summary>
	/// <remarks>
	/// ログインしてないインスタンスでもTOOTを取得できる、Streamingはムリ。
	/// なので、できるかなーと。
	/// </remarks>
	public partial class OtherInstancesTimelineView : ContentView
	{
		public OtherInstancesTimelineView()
		{
			InitializeComponent();
		}
	}
}
