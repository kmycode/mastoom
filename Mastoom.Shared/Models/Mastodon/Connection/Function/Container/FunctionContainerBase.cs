using Mastoom.Shared.Common;
using Mastoom.Shared.Models.Mastodon.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
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

        protected MastodonAuthentication Auth { get; private set; }

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

        protected FunctionContainerBase(ConnectionType type) : this(new MastodonObjectCollection<T>(), type)
        {
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
            this.Auth = auth;
            this.Counter = this.GetFunctionCounter(auth);
            this._function = await this.Counter.IncrementAsync();
            this.OnFunctionAllocated();
            this._function.Updated += this.OnFunctionUpdated;
            this._function.Deleted += this.OnFunctionDeleted;
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
            this._function.Updated -= this.OnFunctionUpdated;
            this.Counter = null;
            this._function = null;
            this.Auth = null;
        }

        public async Task GetNewestItemsAsync()
        {
            try
            {
                var items = (await this._function.GetNewestAsync()).Reverse();

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

        /// <summary>
        /// FunctionからOnUpdateイベントが呼び出される時に発行。
        /// オーバーライド可能だが、コレクションへの追加処理のためにbase呼び出しを強く推奨
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnFunctionUpdated(object sender, ObjectFunctionEventArgs<T> e)
        {
            GuiThread.Run(() =>
            {
                this._objects.Add(e.Object);
            });
        }

        /// <summary>
        /// FunctionからOnDeleteイベントが呼び出される時に発行。
        /// オーバーライド可能だが、コレクションへの追加処理のためにbase呼び出しを強く推奨
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnFunctionDeleted(object sender, ObjectFunctionEventArgs<T> e)
        {
            GuiThread.Run(() =>
            {
                this._objects.RemoveId(e.Object.Id);
            });
        }
    }
}
