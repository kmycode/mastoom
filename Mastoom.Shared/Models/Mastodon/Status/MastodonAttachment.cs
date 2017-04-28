using System;
using Mastonet.Entities;

namespace Mastoom.Shared.Models.Mastodon.Status
{
    /// <summary>
    /// トゥートへの添付メディア
    /// </summary>
	public class MastodonAttachment
	{
        /// <summary>
        /// ID
        /// </summary>
		public int Id { get; }

        /// <summary>
        /// プレビューするためのURI
        /// </summary>
		public string PreviewUrl { get; }

        /// <summary>
        /// トゥートが他インスタンスに投稿されたものであれば、
        /// 他インスタンスにおけるメディアの原URI
        /// </summary>
		public string RemoteUrl { get; }

        /// <summary>
        /// トゥート本文に含めるためのメディアの短縮URI
        /// </summary>
		public string TextUrl { get; }

        /// <summary>
        /// メディアのURI
        /// </summary>
		public string Url { get; }

        /// <summary>
        /// メディアの種類
        /// </summary>
		public AttachmentType Type { get; }

		internal MastodonAttachment(int id, string previewUrl, string remoteUrl, string textUrl, string url, string type)
		{
			this.Id = id;
			this.PreviewUrl = previewUrl;
			this.RemoteUrl = remoteUrl;
			this.TextUrl = textUrl;
			this.Url = url;

            switch (type)
            {
                case "image":
                    this.Type = AttachmentType.Image;
                    break;
                case "video":
                    this.Type = AttachmentType.Video;
                    break;
                case "gifv":
                    this.Type = AttachmentType.GifVideo;
                    break;
                default:
                    this.Type = AttachmentType.Unknown;
                    break;
            }
		}

	}

    /// <summary>
    /// 添付ファイルの種類
    /// </summary>
    public enum AttachmentType
    {
        /// <summary>
        /// 不明
        /// </summary>
        Unknown,

        /// <summary>
        /// 画像
        /// </summary>
        Image,

        /// <summary>
        /// 動画
        /// </summary>
        Video,

        /// <summary>
        /// GIFアニメ
        /// </summary>
        GifVideo,
    }
}
