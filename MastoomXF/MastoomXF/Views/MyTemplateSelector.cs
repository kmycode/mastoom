using System;
using System.Collections.Generic;
using Mastoom.Shared.Models.Mastodon;
using Mastoom.Shared.Models.Mastodon.Connection;
using Xamarin.Forms;

namespace Mastoom.Views
{
    public class MyTemplateSelector : DataTemplateSelector
    {
        private readonly DataTemplate _favouritesTemplate;
        private readonly DataTemplate _notificationsTemplate;

        // Home と Other は複数アカウント対応、つまり可変なので貯めとく
        private readonly IDictionary<string, DataTemplate> _homeTemplates = new Dictionary<string, DataTemplate>();
        private readonly IDictionary<string, DataTemplate> _otherTemplates = new Dictionary<string, DataTemplate>();

        public MyTemplateSelector()
        {
            _favouritesTemplate = new DataTemplate(typeof(FavouritesView));
            _notificationsTemplate = new DataTemplate(typeof(NotificationsView));
        }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            // TODO: Favourtites や Notifications などへの分岐条件となるフラグが
            // MastodonConnection もしくはそれに代わるものに要りそう

            var cnn = item as MastodonConnectionGroup;
            if (cnn == null)
            {
                throw new InvalidOperationException("item is invalid as View");
            }

            if (_homeTemplates.ContainsKey(cnn.Name))
            {
                return _homeTemplates[cnn.Name];
            }

            var template = new DataTemplate(typeof(SingleTimelineView));
            _homeTemplates.Add(cnn.Name, template);

            return template;
        }
    }
}
