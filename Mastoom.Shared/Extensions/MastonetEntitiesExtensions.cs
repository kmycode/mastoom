using System;
using System.Linq;
using Mastoom.Shared.Models.Mastodon.Account;
using Mastoom.Shared.Models.Mastodon.Status;
using Mastoom.Shared.Models.Mastodon.Notification;

namespace Mastoom.Shared
{
    /// <summary>
    /// Mastonet.Entities 関連クラスの拡張メソッド。
    /// 主に Mastoom.Shared.Models.Mastodon.Status などへの変換を行う。
    /// </summary>
    public static class MastonetEntitiesExtensions
	{
        /// <summary>
        /// Mastonet の Status を MastodonStatus に変換する
        /// </summary>
        public static MastodonStatus ToMastodonStatus(this Mastonet.Entities.Status self)
        {
            if (self == null) return null;

            return new MastodonStatus(
                id: self.Id,
                reblog: self.Reblog.ToMastodonStatus(),
                content: self.Content,
                favorited: self.Favourited ?? false,
                reblogged: self.Reblogged ?? false,
                account: self.Account.ToMastodonAccount(),
                mediaAttachments: self.MediaAttachments.Select(x => x.ToMastodonAttachment()),
                tags: self.Tags.Select(x => x.ToMastodonTag()),
                createdAt: self.CreatedAt
            );
        }

        public static MastodonAccount ToMastodonAccount(this Mastonet.Entities.Account self)
        {
            if (self == null) return null;

            return new MastodonAccount(
                id: self.Id,
                accountName: self.AccountName,
                url: self.ProfileUrl,
                displayName: self.DisplayName,
                avatarUrl: self.AvatarUrl
            );
        }

        public static MastodonAttachment ToMastodonAttachment(this Mastonet.Entities.Attachment self)
        {
            if (self == null) return null;

            return new MastodonAttachment(
                id: self.Id,
                previewUrl: self.PreviewUrl,
                remoteUrl: self.RemoteUrl,
                textUrl: self.TextUrl,
                url: self.Url,
                type: self.Type
            );
        }

        public static MastodonTag ToMastodonTag(this Mastonet.Entities.Tag self)
        {
            if (self == null) return null;

            return new MastodonTag(
                name: self.Name,
                url: self.Url
            );
        }

        public static MastodonNotification ToMastodonNotification(this Mastonet.Entities.Notification self)
        {
            if (self == null) return null;

            return new MastodonNotification(
                id: self.Id,
                type: self.Type,
                account: self.Account.ToMastodonAccount(),
                status: self.Status.ToMastodonStatus()
            );
        }
	}
}
