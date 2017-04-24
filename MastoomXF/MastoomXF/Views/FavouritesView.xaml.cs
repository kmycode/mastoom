using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace Mastoom.Views
{
	/// <summary>
	/// お気に入りを表示する View
	/// </summary>
	/// <remarks>
	/// Streaming じゃない気がする。つまり Pull-to-Refresh が必要。
	/// </remarks>
	public partial class FavouritesView : ContentView
	{
		public FavouritesView()
		{
			InitializeComponent();
		}
	}
}
