using System;
using System.Threading.Tasks;

namespace Mastoom.Shared.Common
{
	/// <summary>
	/// System.Threading.Timer クラスが Xamarin.Forms で使えないので分岐するためのラッパー
	/// </summary>
	/// <remarks>
	/// Xamarin.Forms側は Task.Delay 使ってみたけど、これはプラットフォーム非依存だから、
	/// 問題がなければ UWP 側も Task.Delay で共通化してもよいかも。
	/// </remarks>
	public class Timer
	{
#if WINDOWS_UWP
        private readonly System.Threading.Timer _timer;

        public Timer(Action<object> callback, object state, int dueTime, int period)
        {
            _timer = new System.Threading.Timer(s => callback?.Invoke(s), state, dueTime, period);
        }

        public bool Change(int dueTime, int period)
        {
            return _timer.Change(dueTime, period);
        }
#else
		private int _dueTime;
		private int _period;
		public Timer(Action<object> callback, object state, int dueTime, int period)
		{
			_dueTime = dueTime;
			_period = period;
            StartTimer(callback, state);
		}

		void StartTimer(Action<object> callback, object state)
		{
			if (_dueTime < 0 || _period < 0)
			{
				return;
			}

			Task.Run(async () => 
			{
				await Task.Delay(_dueTime);
			   	while(true)
				{
					callback?.Invoke(this);
					await Task.Delay(_period);
    			}
			});
		}

		public bool Change(int dueTime, int period)
		{
			// マイナス値には替えられないようにしちゃったけど大丈夫かな？
			if (dueTime < 0 || period < 0)
			{
				return false;
			}

			_dueTime = dueTime; // 変えても意味ないけど
			_period = period;
			return true;
		}
#endif
	}
}
