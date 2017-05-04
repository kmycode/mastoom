using Mastonet;
using Mastoom.Shared.Models.Mastodon.Connection.Frame;
using Mastoom.Shared.Models.Mastodon.Status;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Mastoom.Shared.Models.Common
{
    /// <summary>
    /// １つのMastodonConnectionで表示するフレームを操作するモデル。
    /// nullであればタイムラインを表示する
    /// </summary>
    public class ConnectionFrameStackModel : INotifyPropertyChanged
    {
        private readonly Stack<ConnectionFrame> frames = new Stack<ConnectionFrame>();

        /// <summary>
        /// 現在表示すべきフレーム。ない（タイムライン表示）場合はnull
        /// </summary>
        public ConnectionFrame CurrentFrame
        {
            get
            {
                return this.frames.FirstOrDefault();
            }
        }

        /// <summary>
        /// 何かフレームを持っているか。ない（タイムライン表示）場合はfalse
        /// </summary>
        public bool IsHaveFrames => this.frames.Count > 0;

        public void Push(ConnectionFrame frame)
        {
            this.frames.Push(frame);
            this.Pushed?.Invoke(this, new EventArgs());
            this.OnPropertyChanged("CurrentFrame");
            this.OnPropertyChanged("IsHaveFrames");
        }

        public ConnectionFrame Pop()
        {
            var item = this.frames.Pop();
            this.Popped?.Invoke(this, new EventArgs());
            this.OnPropertyChanged("CurrentFrame");
            this.OnPropertyChanged("IsHaveFrames");
            return item;
        }

        public ConnectionFrame Peek()
        {
            return this.frames.Peek();
        }

        public event EventHandler Pushed;
        public event EventHandler Popped;
        
        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
