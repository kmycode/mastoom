using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace Mastoom.Shared.Models.Mastodon.Connection
{
    public class MastodonConnectionCollection : ObservableCollection<MastodonConnection>, INotifyPropertyChanged
    {
		#region プロパティ

		/// <summary>
		/// メインに指定された接続
		/// </summary>
		public MastodonConnection Main
		{
			get
			{
				return this._main;
			}
			set
			{
				if (this._main != value)
				{
					this._main = value;
					this.OnPropertyChanged();
				}
			}
		}
		private MastodonConnection _main;
		
		#endregion

#if DEBUG
		public void AddTestConnection()
		{
			this.Add(new MastodonConnection
			{
				InstanceUri = "pawoo.net",
				Name = "Pawoo",
			});
			this.Add(new MastodonConnection
			{
				InstanceUri = "mstdn.jp",
				Name = "Mstdn",
			});
			this.Main = this[0];
		}
#endif

		#region INotifyPropertyChanged

		public new event PropertyChangedEventHandler PropertyChanged;
		protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		#endregion

	}
}
