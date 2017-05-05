using Mastonet;
using Mastoom.Shared.Models.Mastodon.Generic;
using Mastoom.Shared.Models.Mastodon.Status;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mastoom.Shared.Models.Mastodon.Connection.Function
{
    /// <summary>
    /// ステータスの詳細を取得する
    /// </summary>
    class StatusDetailFunction : IConnectionFunction<MastodonObjectWithTag<MastodonStatus, StatusDetailType>>
    {
        public MastodonClient Client { get; set; }

        public int StatusId { get; }

        public event EventHandler<ObjectFunctionUpdateEventArgs<MastodonObjectWithTag<MastodonStatus, StatusDetailType>>> Updated;
        public event EventHandler<ObjectFunctionEventArgs<MastodonObjectWithTag<MastodonStatus, StatusDetailType>>> Deleted;
        public event EventHandler<ObjectFunctionErrorEventArgs> Errored;

        public StatusDetailFunction(int statusId)
        {
            this.StatusId = statusId;
        }

        public async Task<IEnumerable<MastodonObjectWithTag<MastodonStatus, StatusDetailType>>> GetNewestAsync()
        {
            return Enumerable.Empty<MastodonObjectWithTag<MastodonStatus, StatusDetailType>>();
        }

        public async Task<IEnumerable<MastodonObjectWithTag<MastodonStatus, StatusDetailType>>> GetPrevAsync(int maxId)
        {
            return Enumerable.Empty<MastodonObjectWithTag<MastodonStatus, StatusDetailType>>();
        }

        public async Task StartAsync()
        {
            // トゥートそのものを取得する
            var mainStatus = (await this.Client.GetStatus(this.StatusId)).ToMastodonStatus();
            this.OnUpdated(mainStatus, StatusDetailType.Main);

            // 返信元と、トゥートに対する返信の一覧を取得する
            var context = await this.Client.GetStatusContext(this.StatusId);
            foreach (var item in context.Descendants.Select(s => s.ToMastodonStatus()))
            {
                this.OnUpdated(item, StatusDetailType.Descendant);
            }
            foreach (var item in context.Ancestors.Select(s => s.ToMastodonStatus()))
            {
                this.OnUpdated(item, StatusDetailType.Ancestors);
            }
        }

        public async Task StopAsync()
        {
        }

        private void OnUpdated(MastodonStatus status, StatusDetailType type)
        {
            this.Updated?.Invoke(this, new ObjectFunctionUpdateEventArgs<MastodonObjectWithTag<MastodonStatus, StatusDetailType>>
            {
                Object = new MastodonObjectWithTag<MastodonStatus, StatusDetailType>(status, type),
            });
        }
    }

    public enum StatusDetailType
    {
        /// <summary>
        /// メインで表示するステータス
        /// </summary>
        Main,

        /// <summary>
        /// 上位に位置するステータス。
        /// 例：メインステータスの返信先となるステータス
        /// </summary>
        Descendant,

        /// <summary>
        /// 下位に位置するステータス。
        /// 例：メインステータスに対する返信
        /// </summary>
        Ancestors,
    }
}
