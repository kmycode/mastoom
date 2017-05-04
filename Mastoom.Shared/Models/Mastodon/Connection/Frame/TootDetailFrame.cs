using Mastonet;
using Mastoom.Shared.Common;
using Mastoom.Shared.Models.Mastodon.Status;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mastoom.Shared.Models.Mastodon.Connection.Frame
{
    /// <summary>
    /// トゥートの詳細を表示するフレーム
    /// </summary>
    public class TootDetailFrame : ConnectionFrame
    {
        /// <summary>
        /// トゥートのID
        /// </summary>
        private readonly int tootId;

        public MastodonStatus Status
        {
            get
            {
                return this._status;
            }
            set
            {
                if (this._status != value)
                {
                    this._status = value;
                    this.OnPropertyChanged();
                }
            }
        }
        private MastodonStatus _status;

        /// <summary>
        /// トゥートの前に表示するトゥート
        /// </summary>
        public ObservableCollection<MastodonStatus> BeforeStatuses { get; } = new ObservableCollection<MastodonStatus>();

        /// <summary>
        /// トゥートの後に表示するトゥート
        /// </summary>
        public ObservableCollection<MastodonStatus> AfterStatuses { get; } = new ObservableCollection<MastodonStatus>();

        public TootDetailFrame(int id)
        {
            this.tootId = id;
        }

        public override async Task LoadAsync(MastodonClient client)
        {
            await base.LoadAsync(client);

            // トゥートそのものを取得する
            this.Status = (await client.GetStatus(this.tootId)).ToMastodonStatus();

            // 返信元と、トゥートに対する返信の一覧を取得する
            var context = await client.GetStatusContext(this.tootId);
            GuiThread.Run(() =>
            {
                foreach (var item in context.Descendants.Select(s => s.ToMastodonStatus()))
                {
                    this.BeforeStatuses.Add(item);
                }
                foreach (var item in context.Ancestors.Select(s => s.ToMastodonStatus()))
                {
                    this.AfterStatuses.Add(item);
                }
            });
        }
    }
}
