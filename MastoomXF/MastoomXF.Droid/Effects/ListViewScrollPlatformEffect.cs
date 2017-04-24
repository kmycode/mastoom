using System;
using System.Linq;
using Mastoom.Droid.Effects;
using Mastoom.Effects;
using Mastoom.Shared.Models.Mastodon.Status;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ResolutionGroupName("Mastoom")]
[assembly: ExportEffect(typeof(ListViewScrollPlatformEffect), "ListViewScrollEffect")]
namespace Mastoom.Droid.Effects
{
	public class ListViewScrollPlatformEffect : PlatformEffect
	{
		private Android.Widget.ListView _nativeListView;
		private MastodonStatusCollection _collection;

		protected override void OnAttached()
		{
			_nativeListView = this.Control as Android.Widget.ListView;

			_nativeListView.Scroll += ListView_Scroll;

			// うまくうごかないので ListView_Scroll 使う方にする（だから Effects を使わざるをえない）
			//(Element as ListView).ItemDisappearing += (s, e) => 
			//{
			//	var firstItem = _collection?.FirstOrDefault();
			//	if (firstItem == null)
			//	{
			//		return;
			//	}
			//	var disappearItem = e.Item as MastodonStatus;
			//	if (firstItem.Id == disappearItem.Id)
			//	{
			//		// スクロール停止
			//		_collection.EnterPageMode();
			//	}
			//};

			UpdateCollection();
		}

		protected override void OnDetached()
		{
			if (_nativeListView == null)
			{
				return;
			}

			_nativeListView.Scroll -= ListView_Scroll;
		}

		void ListView_Scroll(object sender, Android.Widget.AbsListView.ScrollEventArgs e)
		{
			// 先頭行が隠れるくらいスクロールしたら PageMode にする
			if (e.FirstVisibleItem > 1)
			{
				 // スクロール停止
				_collection.EnterPageMode();
			}
		}

		protected override void OnElementPropertyChanged(System.ComponentModel.PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(e);

			if (e.PropertyName == ListViewScrollEffect.CollectionProperty.PropertyName)
			{
				UpdateCollection();
			}
		}

		void UpdateCollection()
		{
			// 意味があるかわからないけど一応
			if (_collection != null)
			{
				_collection.PageModeExited -= Collection_PageModeExited;
			}

			_collection = ListViewScrollEffect.GetCollection(Element);
			_collection.PageModeExited += Collection_PageModeExited;
		}

		void Collection_PageModeExited(object sender, EventArgs e)
		{
			_nativeListView.SetSelection(0);
		}
	}
}
