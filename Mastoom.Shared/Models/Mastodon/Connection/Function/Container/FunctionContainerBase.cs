using Mastoom.Shared.Common;
using Mastoom.Shared.Models.Mastodon.Generic;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Mastoom.Shared.Models.Mastodon.Connection.Function.Container
{
    abstract class FunctionContainerBase<T> : IFunctionContainer
        where T : MastodonObject
    {
        public IConnectionFunction Function => this._function;
        protected IConnectionFunction<T> _function { get; private set; }

        public ITimelineCollection Objects => this._objects;
        protected MastodonObjectCollection<T> _objects { get; private set; }

        public ConnectionType ConnectionType => this._connectionType;
        private readonly ConnectionType _connectionType;

        /// <summary>
        /// 認証オブジェクトから取得できるカウンタをここに置く。
        /// Allocateで値が入り、Releaseでnullになる
        /// </summary>
        protected ConnectionFunctionCounterBase<T> Counter { get; private set; }

        protected FunctionContainerBase(MastodonObjectCollection<T> collection, ConnectionType type)
        {
            this._objects = collection;
            this._connectionType = type;
        }

        /// <summary>
        /// ConnectionFunctionが手に入った時に呼び出される。
        /// サブクラスからのオーバーライドは任意。
        /// Counter、Functionに値がセットされている状態
        /// </summary>
        protected virtual void OnFunctionAllocated() { }

        /// <summary>
        /// ConnectionFunctionの実行を止めた時に呼び出される。
        /// サブクラスからのオーバーライドは任意。
        /// Counter、Functionに値がセットされている状態
        /// </summary>
        protected virtual void OnFunctionReleased() { }

        public async Task AllocateFunctionAsync(MastodonAuthentication auth)
        {
            this.Counter = this.GetFunctionCounter(auth);
            this._function = await this.Counter.IncrementAsync();
            this.OnFunctionAllocated();
            this._function.Updated += this.Function_OnUpdate;
        }

        /// <summary>
        /// 認証オブジェクトから、サーバと通信を行うためのConnectionFunctionを取り出す
        /// </summary>
        /// <param name="auth">認証オブジェクト</param>
        /// <returns></returns>
        protected abstract ConnectionFunctionCounterBase<T> GetFunctionCounter(MastodonAuthentication auth);

        public async Task ReleaseFunctionAsync()
        {
            await this.Counter.DecrementAsync();
            this.OnFunctionReleased();
            this._function.Updated -= this.Function_OnUpdate;
            this.Counter = null;
            this._function = null;
        }

        public async Task GetNewestItemsAsync()
        {
            try
            {
                var items = await this._function.GetNewestAsync();

                GuiThread.Run(() =>
                {
                    foreach (var item in items)
                    {
                        this._objects.Add(item);
                    }
                });
            }
            catch (Exception e)
            {
                // TODO: とりあえず何もしない
            }
        }

        public async Task GetPrevAsync()
        {
            throw new NotImplementedException();
            //try
            //{
            //    int maxId = -1;
            //    var items = await this._function.GetPrevAsync(maxId);

            //    GuiThread.Run(() =>
            //    {
            //        foreach (var item in items)
            //        {
            //            this._objects.Add(item);
            //        }
            //    });
            //}
            //catch
            //{

            //}
        }

        private void Function_OnUpdate(object sender, ObjectFunctionEventArgs<T> e)
        {
            GuiThread.Run(() =>
            {
                this._objects.Add(e.Object);
            });
        }
    }
}
