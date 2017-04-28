using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace Mastoom.Views
{
    /// <summary>
    /// 「通知」を表示する View
    /// </summary>
    /// <remarks>
    /// 複数のインスタンスにログインしていた場合、
    /// すべてのアカウントの通知をこの View に集約したい（してみたい）。
    /// その場合、アカウント毎に背景色を色分けしてみますか。
    /// あ、Streaming 対応です。
    /// </remarks>
    public partial class NotificationsView : ContentView
    {
        public NotificationsView()
        {
            InitializeComponent();
        }
    }
}
