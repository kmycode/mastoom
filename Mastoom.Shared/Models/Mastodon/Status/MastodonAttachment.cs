using System;
using Mastonet.Entities;

namespace Mastoom.Shared
{
	public class MastodonAttachment
	{
		public int Id { get; }
		public string PreviewUrl { get; }
		public string RemoteUrl { get; }
		public string TextUrl { get; }
		public string Url { get; }
		public string Type { get; }

		public MastodonAttachment(Attachment attachment)
		{
			Id = attachment.Id;
			PreviewUrl = attachment.PreviewUrl;
			RemoteUrl = attachment.RemoteUrl;
			TextUrl = attachment.TextUrl;
			Url = attachment.Url;
			Type = attachment.Type;
		}

	}
}
