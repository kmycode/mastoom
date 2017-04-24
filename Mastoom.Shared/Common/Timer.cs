using System;
namespace Mastoom.Shared.Common
{
	public class Timer
	{
		public Timer( Action<object> callback, object state, int dueTime, int period)
		{
		}

		public bool Change(int dueTime, int period)
		{
			return true;
		}
	}
}
