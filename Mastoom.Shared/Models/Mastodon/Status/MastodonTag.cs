using System;
using Mastonet.Entities;
using Mastoom.Shared.Models.Mastodon.Generic;

namespace Mastoom.Shared
{
	public class MastodonTag : MastodonObject
	{
        /// <summary>
        /// タグ名
        /// </summary>
		public string Name { get; }

        /// <summary>
        /// タグのURI
        /// </summary>
		public string Url { get; }

        public MastodonTag(string name, string url) : base((name + url).GetHashCode())
        {
	        this.Name = name;
	        this.Url = url;
        }
	}
}
