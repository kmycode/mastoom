using System;
using System.Collections.Generic;
using System.Text;

namespace Mastoom.Shared.Models.Mastodon.Generic
{
    /// <summary>
    /// タイムライン上に表示することを想定したインターフェース。
    /// スクロールで表示項目をダイナミックに変化させる
    /// </summary>
    public interface ITimelineCollection
    {
        bool IsPageMode { get; }

        void EnterPageMode();

        void ExitPageMode();

        bool PreviewNextPage();

        void NextPage();

        bool PreviewPrevPage();

        void PrevPage();

        void NewestPage();

        event EventHandler PageModeExited;
    }
}
