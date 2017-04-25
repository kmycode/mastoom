using Mastoom.Shared.Models.Mastodon.Generic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Mastoom.Shared.Models.Mastodon.Status
{
    public class MastodonStatusCollection : MastodonObjectCollection<MastodonStatus>
    {
        protected override void CopyObject(MastodonStatus from, MastodonStatus to)
        {
            from.CopyTo(to);
        }
    }
}
