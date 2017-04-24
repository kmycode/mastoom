using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Mastoom.Shared.Models.Mastodon.Connection.Function
{
    /// <summary>
    /// 他のConnectionFunctionのカウンタを乗っ取って自分のものにする（コンポジションする）クラス。
    /// 他のConnectionFunctionを乗っ取るタイプのConnectionFunctionを登録する時に使う
    /// </summary>
    /// <typeparam name="T">自身のカウンタに格納するConnectionFunctionの型</typeparam>
    /// <typeparam name="COMPO">乗っ取り対象のカウンタが扱うConnectionFunctionの型</typeparam>
    public class ConnectionFunctionCompositionCounter<T, COMPO> : ConnectionFunctionCounterBase<T>
    {
        private readonly ConnectionFunctionCounter<COMPO> compoCounter;

        public ConnectionFunctionCompositionCounter(IConnectionFunction<T> function, ConnectionFunctionCounter<COMPO> compoCounter)
            : base(function)
        {
            this.compoCounter = compoCounter;

            this.compoCounter.CountChanged += (sender, e) =>
            {
                // 乗っ取り対象のカウンタのカウントが0になったときに、勝手に通信を止められたら困る
                if (this.Count > 0)
                {
                    e.CanStop = false;
                }
            };
        }

        public override async Task<IConnectionFunction<T>> IncrementAsync()
        {
            if (this.Count++ == 0 && this.compoCounter.Count == 0)
            {
                await this.compoCounter.Function.StartAsync();
            }
            return this.Function;
        }

        public override async Task DecrementAsync()
        {
            this.Count--;
            if (this.compoCounter.Count == 0 && this.CanStop)
            {
                await this.compoCounter.Function.StopAsync();
            }
        }
    }
}
