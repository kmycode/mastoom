using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Mastoom.Shared.Models.Mastodon.Generic
{
    public class MastodonObjectCollection<T> : ReadOnlyObservableCollection<T>, INotifyPropertyChanged
        where T : MastodonObject
    {
        protected ObservableCollection<T> Collection { get; }

        /// <summary>
        /// PageModeであるか（画面をスクロール途中のモード）
        /// </summary>
        public bool IsPageMode
        {
            get
            {
                return this._isPageMode;
            }
            private set
            {
                if (this._isPageMode != value)
                {
                    this._isPageMode = value;
                    this.OnPropertyChanged();
                }
            }
        }
        private bool _isPageMode;

        /// <summary>
        /// ステータスを追加する時に適用するフィルタ
        /// </summary>
        public Predicate<T> Filter { get; set; } = (status) => true;

        /// <summary>
        /// 画面スクロールなどに対応して、実際に表示する要素
        /// </summary>
        public ObservableCollection<T> DynamicLimited { get; } = new ObservableCollection<T>();

        public MastodonObjectCollection() : this(new ObservableCollection<T>())
		{
        }

        private MastodonObjectCollection(ObservableCollection<T> collection) : base(collection)
		{
            this.Collection = collection;
        }

        /// <summary>
        /// オブジェクトの内容をコピーする
        /// </summary>
        /// <param name="from">コピー元</param>
        /// <param name="to">コピー先</param>
        protected virtual void CopyObject(T from, T to) { }

        public virtual void Add(T obj)
        {
            if (!this.Filter(obj))
            {
                return;
            }

            var existing = this.FindById(obj.Id);
            if (existing != null)
            {
                this.CopyObject(obj, existing);
            }
            else
            {
                this.ForceAdd(obj);
            }
        }

        protected void ForceAdd(T obj)
        {
            this.Collection.Insert(0, obj);
            if (!this.IsPageMode)
            {
                this.DynamicLimited.Insert(0, obj);
                if (this.DynamicLimited.Count > 100)
                {
                    this.DynamicLimited.RemoveAt(100);
                }
            }

            // メモリに保持する件数の限界
            if (this.Count > 2500)
            {
                this.Collection.RemoveAt(2500);
            }
        }

        /// <summary>
        /// PageModeに入る
        /// </summary>
        public void EnterPageMode()
        {
            if (this.IsPageMode) return;

            this.IsPageMode = true;
            this.PreviewNextPage(true);
        }

        /// <summary>
        /// PageModeを解除する
        /// </summary>
        public void ExitPageMode()
        {
            if (!this.IsPageMode) return;

            this.IsPageMode = false;
            this.NewestPage();

            this.PageModeExited?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// 次のページ（古い投稿）を画面に表示させる。
        /// 目的は、次のページを表示することによって、画面のスクロール量がどれだけ増えるか計算すること
        /// </summary>
        /// <param name="isEnter">PageMode開始直後に呼び出したものであるか。このパラメータは通常は本クラス内部からの呼び出し時に設定される</param>
        /// <returns></returns>
        public bool PreviewNextPage(bool isEnter = false)
        {
            var last = this.DynamicLimited.LastOrDefault();
            var firstIndex = isEnter ? 0 : (last != null ? this.IndexOf(last) + 1 : 0);
            if (firstIndex >= this.Count) return false;

            int num = 50;
            for (int i = firstIndex; i < this.Count && num > 0; i++, num--)
            {
                if (this.DynamicLimited.Contains(this[i]) == false)
                {
                    this.DynamicLimited.Add(this[i]);
                }
            }

            return true;
        }

        /// <summary>
        /// 前のページ（新しい投稿）を画面から削除する。
        /// 次のページ（古い投稿）を表示させた後に呼び出す
        /// </summary>
        public void NextPage()
        {
            while (this.DynamicLimited.Count > 100)
            {
                this.DynamicLimited.RemoveAt(0);
            }
        }

        /// <summary>
        /// 前のページ（新しい投稿）を画面に表示させる。
        /// 目的は、前のページを表示することによって、画面のスクロール量がどれだけ増えるか計算すること
        /// </summary>
        /// <returns></returns>
        public bool PreviewPrevPage()
        {
            var first = this.DynamicLimited.FirstOrDefault();
            var lastIndex = first != null ? this.IndexOf(first) - 1 : 0;
            if (lastIndex < 5)     // TL流れるの速すぎてスクロールだけでの復帰が難しいので0ではない
            {
                return false;
            }

            int num = 50;
            for (int i = lastIndex; i >= 0 && num > 0; i--, num--)
            {
                if (this.DynamicLimited.Contains(this[i]) == false)
                {
                    this.DynamicLimited.Insert(0, this[i]);
                }
            }

            return true;
        }

        /// <summary>
        /// 次のページ（古い投稿）を画面から削除する。
        /// 前のページ（新しい投稿）を表示させた後に呼び出す
        /// </summary>
        public void PrevPage()
        {
            while (this.DynamicLimited.Count > 100)
            {
                this.DynamicLimited.RemoveAt(100);
            }
        }

        /// <summary>
        /// 最新のページを表示する
        /// </summary>
        public void NewestPage()
        {
            this.DynamicLimited.Clear();
            var first100 = this.Take(100);
            foreach (var item in first100)
            {
                this.DynamicLimited.Add(item);
            }
        }

        /// <summary>
        /// IDからオブジェクトを検索して返す
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private T FindById(int id)
        {
            return this.SingleOrDefault(item => item.Id == id);
        }

        /// <summary>
        /// Published when exited page mode
        /// </summary>
        public event EventHandler PageModeExited;

        #region INotifyPropertyChanged

        public new event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
