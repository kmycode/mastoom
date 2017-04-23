using System;
using Mastonet.Entities;

namespace Mastoom.Shared
{
	public class MastodonTag
	{
		public string Name { get; }
		public string Url { get; }

		public MastodonTag(Tag tag)
		{
			Name = tag.Name;
			Url = tag.Url;
		}
	}
}
