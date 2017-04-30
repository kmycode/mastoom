using System;
using System.Threading.Tasks;
using Akavache;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;

namespace Mastoom.Shared.Repositories
{
	public class OAuthAccessTokenRepository
	{
		public OAuthAccessTokenRepository()
		{
		}

        public Task Save(string instanceUri, string accessToken)
        {
            return BlobCache.LocalMachine.InsertObject(instanceUri, accessToken).ToTask().ContinueWith(_ => 
            {
                BlobCache.LocalMachine.Flush();
            });
        }

        internal Task<string> Load(string instanceUri)
        {
            return BlobCache.LocalMachine.GetOrCreateObject<string>(instanceUri, () => string.Empty).ToTask();
        }
    }
}
