using Mastoom.Shared.Models.Mastodon.Status;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Mastoom.Shared.Models.Mastodon.Connection.Function
{
    /// <summary>
    /// 接続のファンクション。
    /// 公開タイムラインなどのStreamingと、
    /// お気に入り・フォロワー一覧などStreamingのないAPIとの通信を
    /// まとめて抽象化するインターフェース
    /// </summary>
    public interface IConnectionFunction
    {
        /// <summary>
        /// 受信を開始する
        /// </summary>
        Task StartAsync();

        /// <summary>
        /// 受信を停止する
        /// </summary>
        Task StopAsync();
    }

    /// <summary>
    /// IConnectionFunctionに型をつける
    /// </summary>
    /// <typeparam name="T">このインターフェースがサーバから取得するオブジェクトの型。例：タイムラインと接続するクラスであればMastodonStatus</typeparam>
    public interface IConnectionFunction<T> : IConnectionFunction
    {
        /// <summary>
        /// データが新規追加、または更新された時に発行
        /// </summary>
        event EventHandler<ObjectFunctionUpdateEventArgs<T>> Updated;

        /// <summary>
        /// データが削除された時に発行
        /// </summary>
        event EventHandler<ObjectFunctionEventArgs<T>> Deleted;

        /// <summary>
        /// エラーが発生した時に発行
        /// </summary>
        event EventHandler<ObjectFunctionErrorEventArgs> Errored;

        /// <summary>
        /// １まとまり前のデータをサーバから取得する
        /// </summary>
        Task<IEnumerable<T>> GetPrevAsync(int maxId);

        /// <summary>
        /// １まとまり分、最新のデータをサーバから取得する
        /// </summary>
        Task<IEnumerable<T>> GetNewerAsync();
    }

    public class ObjectFunctionEventArgs<T> : EventArgs
    {
        public T Object { get; set; }
    }

    public class ObjectFunctionUpdateEventArgs<T> : ObjectFunctionEventArgs<T>
    {
    }

    public class ObjectFunctionErrorEventArgs : EventArgs
    {
        public ObjectFunctionErrorType Type { get; set; }
        public Exception Exception { get; set; }
    }

    /// <summary>
    /// IConnectionFunctionがエラーを返した時、
    /// そのエラーがどのタイミングで発生したどういう種類のエラーなのか
    /// </summary>
    public enum ObjectFunctionErrorType
    {
        /// <summary>
        /// 受信開始時にエラー発生
        /// </summary>
        StartError,
    }
}
