using Mastoom.Shared.Models.Mastodon.Generic;
using Mastoom.Shared.Models.Mastodon.Status;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mastoom.Shared.Models.Mastodon.Connection.Function.Container
{
    class StatusDetailFunctionContainer : FunctionContainerBase<MastodonObjectWithTag<MastodonStatus, StatusDetailType>>
    {
        /// <remark>
        /// トゥートのIDによって返される結果が変わってくる。
        /// 同じデータを２回以上とらないようにするというConnectionFunctionCounterの目的にそくわないので
        /// 認証オブジェクトではなくコンテナの管理下に置く
        /// </remark>
        private ConnectionFunctionCounter<MastodonObjectWithTag<MastodonStatus, StatusDetailType>> counter;

        private int statusId;

        public StatusDetailFunctionContainer(int statusId) : base(ConnectionType.StatusDetail)
        {
            this.statusId = statusId;
        }

        protected override ConnectionFunctionCounterBase<MastodonObjectWithTag<MastodonStatus, StatusDetailType>>
            GetFunctionCounter(MastodonAuthentication auth)
        {
            if (this.counter == null)
            {
                this.counter = new ConnectionFunctionCounter<MastodonObjectWithTag<MastodonStatus, StatusDetailType>>(
                    new StatusDetailFunction(this.statusId)
                    {
                        Client = auth.Client,
                    }
                );
            }
            return this.counter;
        }
    }
}
