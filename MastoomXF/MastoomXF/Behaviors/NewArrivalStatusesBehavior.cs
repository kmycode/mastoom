using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Mastoom.Shared.Models.Common;
using Mastoom.Shared.Models.Mastodon.Status;
using Xamarin.Forms;

namespace Mastoom.Behaviors
{
	/// <summary>
	/// 「n件の新着」ボタンに関する Behavior
	/// </summary>
	/// <remarks>
	/// 1. ボタンを押した時に、表示してないTOOTを表示してListViewのTopへ移動しPageModeを脱出。<br/>
	/// 2. ボタンのタイトルの n件 のところを更新
	/// </remarks>
	public class NewArrivalStatusesBehavior : BindingContextPassingBehavior<Button>
	{
		#region 依存プロパティ

		public static readonly BindableProperty CollectionProperty =
			BindableProperty.Create(
				propertyName: "Collection",
				returnType: typeof(MastodonStatusCollection),
				declaringType: typeof(NewArrivalStatusesBehavior),
				defaultValue: (MastodonStatusCollection)null,
				propertyChanged: OnCollectionChanged
			);

		public MastodonStatusCollection Collection
		{
			get
			{
				return (MastodonStatusCollection)GetValue(CollectionProperty);
			}
			set
			{
				SetValue(CollectionProperty, value);
			}
		}

		static void OnCollectionChanged(BindableObject bindable, object oldValue, object newValue)
		{
			var behivior = bindable as NewArrivalStatusesBehavior;
			if (behivior == null)
			{
				return;
			}

			var oldColl = oldValue as INotifyCollectionChanged;
			if (oldColl != null)
			{
				oldColl.CollectionChanged -= behivior.Collection_CollectionChanged;
			}

			var newColl = newValue as INotifyCollectionChanged;
			if (newColl != null)
			{
				newColl.CollectionChanged += behivior.Collection_CollectionChanged;
			}
		}

		void Collection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (this.Collection == null)
			{
				AssociatedObject.Text = "新着がないぞ？";
				return;
			}

			var count = Collection.Count - Collection.DynamicLimited.Count;
			AssociatedObject.Text = $"{count}件の新着";
		}

		#endregion

		protected override void OnAttachedTo(Button bindable)
		{
			base.OnAttachedTo(bindable);

			AssociatedObject.Clicked += Button_Clicked;
		}

		protected override void OnDetachingFrom(Button bindable)
		{
			AssociatedObject.Clicked -= Button_Clicked;
			base.OnDetachingFrom(bindable);
		}

		void Button_Clicked(object sender, EventArgs e)
		{
			if (this.Collection != null)
			{
				this.Collection.PerformPrevPage();
				this.Collection.ExitPageMode();
			}
		}
	}
}
