using System;
using System.Collections.Generic;
using System.Text;

namespace Mastoom.Shared.Models.Mastodon.Generic
{
    public abstract class MastodonObject
    {
        public int Id { get; }

        protected MastodonObject(int id)
        {
            this.Id = id;
        }
    }
}
