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

        /// <summary>
        /// １まとまり前のデータをサーバから取得する
        /// </summary>
        Task GetPrevAsync();

        /// <summary>
        /// １まとまり分、最新のデータをサーバから取得する
        /// </summary>
        Task GetNewerAsync();
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
    }

    public class ObjectFunctionEventArgs<T> : EventArgs
    {
        public T Object { get; set; }
    }

    public class ObjectFunctionUpdateEventArgs<T> : ObjectFunctionEventArgs<T>
    {
        public ObjectFunctionAdditionPosition Direction { get; set; }
                        = ObjectFunctionAdditionPosition.Top;
    }

    public class ObjectFunctionErrorEventArgs : EventArgs
    {
        public ObjectFunctionErrorType Type { get; set; }
        public Exception Exception { get; set; }
    }

    /// <summary>
    /// IConnectionFunctionがサーバから取得したオブジェクトについて、
    /// もし新しく追加されたデータであれば、そのデータを追加する位置
    /// </summary>
    public enum ObjectFunctionAdditionPosition
    {
        /// <summary>
        /// 上から順に追加
        /// </summary>
        Top,
        
        /// <summary>
        /// 下から順に追加
        /// </summary>
        Bottom,
    }

    /// <summary>
    /// IConnectionFunctionの種別。
    /// １つの種別あたりただ１つのインスタンスを１つの認証ごとに保持するので、
    /// １つの認証から同じ種別の接続を複数作らないよう、
    /// 認証オブジェクトと他のオブジェクトとのやり取り時に使用する
    /// </summary>
    public enum ConnectionFunctionType
    {
        PublicTimeline,
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
