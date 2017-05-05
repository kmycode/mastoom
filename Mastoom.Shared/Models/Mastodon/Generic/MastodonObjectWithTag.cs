using System;
using System.Collections.Generic;
using System.Text;

namespace Mastoom.Shared.Models.Mastodon.Generic
{
    /// <summary>
    /// マストドンのオブジェクトに、任意のユーザデータ（タグ）をつけたもの
    /// </summary>
    /// <typeparam name="O">MastodonObjectの型</typeparam>
    /// <typeparam name="T">タグの型</typeparam>
    public class MastodonObjectWithTag<O, T> : MastodonObject where O : MastodonObject
    {
        public T Tag { get; }

        public O Object { get; }

        public MastodonObjectWithTag(O obj, T tag) : base(0)
        {
            this.Object = obj;
            this.Tag = tag;
        }
    }
}
