using System;
using System.Threading.Tasks;
using Akavache;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;

namespace Mastoom.Shared.Repositories
{
    /// <summary>
    /// OAuth 認証済みの AccessToken を取得または保存するクラス
    /// </summary>
	public class OAuthAccessTokenRepository
	{
        /// <summary>
        /// AccessToken を Instance URI と紐付けて保存する
        /// </summary>
        /// <returns>The save.</returns>
        /// <param name="instanceUri">Instance URI.</param>
        /// <param name="accessToken">Access token.</param>
        public Task Save(string instanceUri, string accessToken)
        {
            return BlobCache.LocalMachine.InsertObject(instanceUri, accessToken).ToTask().ContinueWith(_ => 
            {
                BlobCache.LocalMachine.Flush();
            });
        }

        /// <summary>
        /// Instance URI に紐付いた AccessToken を読み出す。存在なかったら空文字。
        /// </summary>
        /// <returns>The load.</returns>
        /// <param name="instanceUri">Instance URI.</param>
        public Task<string> Load(string instanceUri)
        {
            return BlobCache.LocalMachine.GetOrCreateObject<string>(instanceUri, () => string.Empty).ToTask();
        }
    }
}
