using System;
using System.Linq;
using Mastoom.Shared.Models.Mastodon.Account;
using Mastoom.Shared.Models.Mastodon.Status;

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
            var status = self;

            // ブーストされたトゥートの場合でも、ブーストした人が取得したIDを設定する
            // （元のトゥートとブースト分のトゥート、両方同じTLに表示するため）
            var id = status.Id;

            var boostedId = 0;
            var isBoost = false;

            // ブーストされたトゥートの場合、ここから先はブースト対象のトゥートのデータを設定する
            if (self.Reblog != null)
            {
                boostedId = status.Reblog.Id;
                status = self.Reblog;
                isBoost = true;
			}

            return new MastodonStatus(
                id,
                boostedId,
                self.Content, self.Favourited ?? false, isBoost,
                self.Account.ToMastodonAccount(),
                self.MediaAttachments.Select(x => new MastodonAttachment(x)),
                self.Tags.Select(x => new MastodonTag(x))
            );
        }

        public static MastodonAccount ToMastodonAccount(this Mastonet.Entities.Account self)
        {
            var left = self?.AccountName?.Split('@')?.ElementAtOrDefault(1);
            var right = self?.ProfileUrl?.Split('/')?.ElementAt(2);

            return new MastodonAccount(
                self.Id,
                self.AccountName,
                left ?? right,
                !self.AccountName?.Contains("@") ?? false,
                self.DisplayName,
                self.AvatarUrl
            );
            
        }
	}
}
