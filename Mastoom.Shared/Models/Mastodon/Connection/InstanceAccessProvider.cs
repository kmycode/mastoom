using Mastonet;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Mastoom.Shared.Models.Mastodon.Connection
{
	/// <summary>
	/// 各インスタンスへのアクセス情報を提供
	/// </summary>
    static class InstanceAccessProvider
    {
		public static InstanceAccessData GetWithInstanceUri(string uri)
		{
			if (uri == "pawoo.net")
			{
				return new InstanceAccessData
				{
					ClientId = "4a5f3c8a86715616a2f90403e1e543db808ae3ee601d53c468368dedeb4337bb",
					ClientSecret = "a6b50a72d96a7f956ffc00a1504e314be487f8026a651b5a4903c9c205650802",
				};
			}
			else if (uri == "mstdn.jp")
			{
				return new InstanceAccessData
				{
					ClientId = "aab460c852b5b1db3370934353b587170c7b741972195f6fda4afa6b91dab3fc",
					ClientSecret = "02267c02950cab8f2fab731376e56bb6015b790c1344e644a102b32b98fd9606",
					StreamingInstance = "streaming.mstdn.jp",
				};
			}
            else if (uri == "mastodon.cloud")
            {
                return new InstanceAccessData
                {
                    ClientId = "3b7a7f9a3e243aaaaa19d6fcdf18a090c34434f1a510af49d520f9b0a6533ae7",
                    ClientSecret = "20e436f32ab88be72e2ff2d0492f02dbf624cae55018d58768ae0fa8aff89c54"
                };
            }

			return null;
		}

		public class InstanceAccessData
		{
			public string ClientId { get; set; }
			public string ClientSecret { get; set; }
			public string StreamingInstance { get; set; }
		}
    }
}
