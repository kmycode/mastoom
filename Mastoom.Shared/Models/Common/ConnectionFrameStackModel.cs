using Mastonet;
using Mastoom.Shared.Models.Mastodon;
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
        private readonly Stack<MastodonConnection> frames = new Stack<MastodonConnection>();

        /// <summary>
        /// 現在表示すべきフレーム。ない（タイムライン表示）場合はnull
        /// </summary>
        public MastodonConnection CurrentFrame
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

        public void Push(MastodonConnection frame)
        {
            this.frames.Push(frame);
            this.Pushed?.Invoke(this, new ConnectionFrameStackChangedEventArgs(frame));
            this.OnPropertyChanged("CurrentFrame");
            this.OnPropertyChanged("IsHaveFrames");
        }

        public MastodonConnection Pop()
        {
            var item = this.frames.Pop();
            this.Popped?.Invoke(this, new ConnectionFrameStackChangedEventArgs(item));
            this.OnPropertyChanged("CurrentFrame");
            this.OnPropertyChanged("IsHaveFrames");
            return item;
        }

        public MastodonConnection Peek()
        {
            return this.frames.Peek();
        }

        public event EventHandler<ConnectionFrameStackChangedEventArgs> Pushed;
        public event EventHandler<ConnectionFrameStackChangedEventArgs> Popped;
        
        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }

    public class ConnectionFrameStackChangedEventArgs : EventArgs
    {
        public MastodonConnection Frame { get; }

        public ConnectionFrameStackChangedEventArgs(MastodonConnection frame)
        {
            this.Frame = frame;
        }
    }
}
