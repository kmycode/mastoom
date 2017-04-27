using Mastoom.Shared.Models.Mastodon.Generic;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Mastoom.Shared.Models.Mastodon.Connection.Function.Container
{
    /// <summary>
    /// ConnectionFunctionとその結果を受取るコレクションを
    /// 扱うためのロジックを抽象化したインターフェース
    /// </summary>
    interface IFunctionContainer
    {
        /// <summary>
        /// 接続の種類
        /// </summary>
        ConnectionType ConnectionType { get; }

        /// <summary>
        /// ConnectionFunctionが取得できる。
        /// Allocateによって値が割り当てられ、Releaseによってnullになる
        /// </summary>
        IConnectionFunction Function { get; }

        /// <summary>
        /// ConnectionFunctionの実行結果が格納されるコレクション
        /// </summary>
        ITimelineCollection Objects { get; }

        /// <summary>
        /// ConnectionFunctionを割り当てる。
        /// このメソッドの実行後、Functionに何かのオブジェクトが割り当てられる。
        /// なお、Functionにすでに値が割り当てられていた時の動作は未定（各FunctionContainerの仕様に依存）
        /// </summary>
        /// <param name="auth">インスタンスの認証情報</param>
        /// <returns></returns>
        Task AllocateFunctionAsync(MastodonAuthentication auth);

        /// <summary>
        /// ConnectionFunctionを解放する。
        /// このメソッドの実行後、Functionはnullになる
        /// </summary>
        /// <returns></returns>
        Task ReleaseFunctionAsync();

        /// <summary>
        /// 最新のアイテムをサーバから取得する
        /// </summary>
        /// <returns></returns>
        Task GetNewestItemsAsync();

        /// <summary>
        /// 一つ前のアイテムをサーバから取得する。
        /// 自動的に、一番古いアイテムの一つ前までのデータが取得され、配列に入る
        /// </summary>
        /// <returns></returns>
        Task GetPrevAsync();
    }
}
