using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Mastoom.Shared.Models.Mastodon.Connection.Function
{
    public abstract class ConnectionFunctionCounterBase<T>
    {
        public IConnectionFunction<T> Function { get; }

        /// <summary>
        /// DecrementでCountが0になったら、サーバとの通信を止めてもかまわないか
        /// </summary>
        protected bool CanStop { get; private set; }

        public int Count
        {
            get
            {
                return this._count;
            }
            set
            {
                if (this._count != value)
                {
                    this._count = value;

                    var e = new ConnectionFunctionCountChangedEventArgs(value);
                    this.CountChanged?.Invoke(this, e);
                    this.CanStop = e.CanStop;
                }
            }
        }
        private int _count;

        public ConnectionFunctionCounterBase(IConnectionFunction<T> function)
        {
            this.Function = function;
        }

        /// <summary>
        /// 接続数を１増やす
        /// </summary>
        /// <returns></returns>
        public abstract Task<IConnectionFunction<T>> IncrementAsync();

        /// <summary>
        /// 接続数を１減らす
        /// </summary>
        public abstract Task DecrementAsync();

        /// <summary>
        /// ConnectionFunctionのCountが変更された時に発行
        /// </summary>
        public event EventHandler<ConnectionFunctionCountChangedEventArgs> CountChanged;
    }

    public class ConnectionFunctionCountChangedEventArgs : EventArgs
    {
        public int Count { get; }

        /// <summary>
        /// Countが0になったときに、サーバとの通信を止めてもかまわないか
        /// </summary>
        public bool CanStop { get; set; } = true;

        public ConnectionFunctionCountChangedEventArgs(int count)
        {
            this.Count = count;
        }
    }

    /// <summary>
    /// 接続されているファンクションのカウンタ。
    /// 内部で保持する変数が0以外になったら接続を自動的に開始し
    /// 0になったら接続を自動的に停止する
    /// </summary>
    public class ConnectionFunctionCounter<T> : ConnectionFunctionCounterBase<T>
    {
        public ConnectionFunctionCounter(IConnectionFunction<T> function) : base(function)
        {
        }

        public override async Task<IConnectionFunction<T>> IncrementAsync()
        {
            if (this.Count++ == 0)
            {
                await this.Function.StartAsync();
            }
            return this.Function;
        }

        public override async Task DecrementAsync()
        {
            this.Count--;
            if (this.Count == 0 && this.CanStop)
            {
                await this.Function.StopAsync();
            }
        }
    }
}
