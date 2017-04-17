using System;
using System.Collections.Generic;
using System.Text;

namespace Mastoom.Shared.Common
{
    static class GuiThread
    {
		public static void Run(Action action)
		{
#if WINDOWS_UWP
			Windows.ApplicationModel.Core.CoreApplication.MainView?.CoreWindow?.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
			{
				action();
			});
#endif
		}
    }
}
