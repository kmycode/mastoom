using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Mastoom.Shared.Models.Mastodon.Connection.Function
{
    /// <summary>
    /// 接続されているファンクションのカウンタ。
    /// 内部で保持する変数が0以外になったら接続を自動的に開始し
    /// 0になったら接続を自動的に停止する
    /// </summary>
    public class ConnectionFunctionCounter<T>
    {
        private IConnectionFunction<T> function;

        public int Count { get; private set; }

        public ConnectionFunctionCounter(IConnectionFunction<T> function)
        {
            this.function = function;
        }

        /// <summary>
        /// 接続数を１増やす
        /// </summary>
        /// <returns></returns>
        public async Task<IConnectionFunction<T>> IncrementAsync()
        {
            if (this.Count++ == 0)
            {
                await this.function.StartAsync();
            }
            return this.function;
        }

        /// <summary>
        /// 接続数を１減らす
        /// </summary>
        public async Task DecrementAsync()
        {
            if (--this.Count == 0)
            {
                await this.function.StopAsync();
            }
        }
    }
}
