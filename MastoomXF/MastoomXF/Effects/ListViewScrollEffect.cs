using System;
using System.Linq;
using Mastoom.Shared.Models.Mastodon.Status;
using Xamarin.Forms;

namespace Mastoom.Effects
{
	public static class ListViewScrollEffect
	{
		public static readonly BindableProperty CollectionProperty =
			BindableProperty.CreateAttached(
				propertyName: "Collection",
				returnType: typeof(MastodonStatusCollection),
				declaringType: typeof(ListViewScrollEffect),
				defaultValue: (MastodonStatusCollection)null,
				propertyChanged: CollectionChanged
			);

		public static void SetCollection(BindableObject view, MastodonStatusCollection value)
		{
			view.SetValue(CollectionProperty, value);
		}

		public static MastodonStatusCollection GetCollection(BindableObject view)
		{
			return (MastodonStatusCollection)view.GetValue(CollectionProperty);
		}

		private static void CollectionChanged(BindableObject bindable, object oldValue, object newValue)
		{
			var view = bindable as View;
			if (view == null)
				return;
			
			if (newValue is MastodonStatusCollection)
			{
				view.Effects.Add(new ListViewScrollRoutingEffect());
			}
			else
			{
				var toRemove = view.Effects.FirstOrDefault(e => e is ListViewScrollRoutingEffect);
				if (toRemove != null)
					view.Effects.Remove(toRemove);
			}
		}
	}
}