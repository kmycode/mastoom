using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Mastoom.Shared.Models.Mastodon.Status
{
    public class MastodonStatusCollection : ReadOnlyObservableCollection<MastodonStatus>, INotifyPropertyChanged
    {
		private ObservableCollection<MastodonStatus> collection;

        /// <summary>
        /// Page mode (during scrolling timeline)
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
        /// 画面スクロールなどに対応して、実際に表示する要素
        /// </summary>
        public ObservableCollection<MastodonStatus> DynamicLimited { get; } = new ObservableCollection<MastodonStatus>();

		public MastodonStatusCollection() : this(new ObservableCollection<MastodonStatus>())
		{
		}

		private MastodonStatusCollection(ObservableCollection<MastodonStatus> collection) : base(collection)
		{
			this.collection = collection;
		}

		public void Add(MastodonStatus status)
		{
			var existing = this.FindById(status.Id);
			if (existing != null)
			{
				status.CopyTo(existing);
			}
			else
			{
				this.collection.Insert(0, status);
                if (!this.IsPageMode)
                {
                    this.DynamicLimited.Insert(0, status);
                    if (this.DynamicLimited.Count > 200)
                    {
                        this.DynamicLimited.RemoveAt(200);
                    }
                }

                // メモリに保持する件数の限界
                if (this.Count > 3000)
                {
                    this.collection.RemoveAt(3000);
                }
			}
		}

        public void EnterPageMode()
        {
            if (this.IsPageMode) return;

            this.IsPageMode = true;
            this.PerformNextPage(true);
        }

        public void ExitPageMode()
        {
            if (!this.IsPageMode) return;

            this.IsPageMode = false;
            this.NewestPage();

            this.PageModeExited?.Invoke(this, new EventArgs());
        }

        public void PerformNextPage(bool isEnter = false)
        {
            var last = this.DynamicLimited.LastOrDefault();
            var firstIndex = isEnter ? 0 : (last != null ? this.IndexOf(last) + 1 : 0);
            if (firstIndex >= this.Count) return;

            int num = 100;
            for (int i = firstIndex; i < this.Count && num > 0; i++, num--)
            {
                if (this.DynamicLimited.Contains(this[i]) == false)
                {
                    this.DynamicLimited.Add(this[i]);
                }
            }
        }

        public void NextPage()
        {
            while (this.DynamicLimited.Count > 200)
            {
                this.DynamicLimited.RemoveAt(0);
            }
        }

        public bool PerformPrevPage()
        {
            var first = this.DynamicLimited.FirstOrDefault();
            var lastIndex = first != null ? this.IndexOf(first) - 1 : 0;
            if (lastIndex < 5)     // TL流れるの速すぎてスクロールだけでの復帰が難しいので0ではない
            {
                return false;
            }

            int num = 100;
            for (int i = lastIndex; i >= 0 && num > 0; i--, num--)
            {
                if (this.DynamicLimited.Contains(this[i]) == false)
                {
                    this.DynamicLimited.Insert(0, this[i]);
                }
            }

            return true;
        }

        public void PrevPage()
        {
            while (this.DynamicLimited.Count > 200)
            {
                this.DynamicLimited.RemoveAt(200);
            }
        }

        public void NewestPage()
        {
            this.DynamicLimited.Clear();
            var first100 = this.Take(200);
            foreach (var item in first100)
            {
                this.DynamicLimited.Add(item);
            }
        }

		private MastodonStatus FindById(int id)
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
