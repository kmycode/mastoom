using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Mastoom.Shared.Models.Mastodon.Connection
{
    /// <summary>
    /// マストドンへの接続のグループ
    /// </summary>
    public class MastodonConnectionGroup : INotifyPropertyChanged
    {
        /// <summary>
        /// このグループに所属できる最大接続数
        /// </summary>
        private const int MaxConnectionCount = 5;

        private readonly ReadOnlyObservableCollection<MastodonConnection> _connectionsCollection;
        private readonly ObservableCollection<MastodonConnection> _connections = new ObservableCollection<MastodonConnection>();

        /// <summary>
        /// このグループに登録された接続一覧
        /// </summary>
        public ReadOnlyObservableCollection<MastodonConnection> Connections => this._connectionsCollection;

        /// <summary>
        /// このグループに所属する接続のうち、現在アクティブになっている接続
        /// </summary>
        public MastodonConnection Activated
        {
            get
            {
                return this._activated;
            }
            set
            {
                if (this._activated != value)
                {
                    this._activated = value;
                    this.OnPropertyChanged();
                }
            }
        }
        private MastodonConnection _activated;

        /// <summary>
        /// このグループにつけられた名前
        /// </summary>
        public string Name
        {
            get
            {
                return this._name;
            }
            set
            {
                if (this._name != value)
                {
                    this._name = value;
                    this.OnPropertyChanged();
                }
            }
        }
        private string _name;

        public MastodonConnectionGroup()
        {
            this._connectionsCollection = new ReadOnlyObservableCollection<MastodonConnection>(this._connections);
        }

        /// <summary>
        /// 新しい接続をグループに追加しようとする
        /// </summary>
        /// <param name="connection"></param>
        /// <returns>グループへの追加が成功したらtrue。何らかの理由で失敗すればfalse</returns>
        public bool TryAdd(MastodonConnection connection)
        {
            if (this._connections.Count >= MaxConnectionCount)
            {
                return false;
            }
            this._connections.Add(connection);

            if (this._connections.Count == 1)
            {
                this.Activated = connection;
            }

            return true;
        }

        public void Remove(MastodonConnection connection)
        {
            if (this.Activated == connection)
            {
                this.Activated = null;
            }
            this._connections.Remove(connection);
        }

        public void RemoveAt(int i)
        {
            if (this.Activated == this._connections.ElementAt(i))
            {
                this.Activated = null;
            }
            this._connections.RemoveAt(i);
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
