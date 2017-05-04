using Mastonet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Mastoom.Shared.Models.Mastodon.Connection.Frame
{
    /// <summary>
    /// 画面に表示するフレームを表現するモデル
    /// </summary>
    public abstract class ConnectionFrame : INotifyPropertyChanged
    {
        /// <summary>
        /// Mastodonと接続して、必要な情報をサーバから取り込む
        /// </summary>
        /// <param name="client">接続するためのクライアント</param>
        /// <returns></returns>
        public virtual async Task LoadAsync(MastodonClient client) { }
        
        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
