﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace Mastoom.Shared.Models.Mastodon.Account
{
    public class MastodonAccount : INotifyPropertyChanged
    {
		/// <summary>
		/// ID
		/// </summary>
		public int Id { get; }

		/// <summary>
		/// 名前
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

		/// <summary>
		/// アイコンのURI
		/// </summary>
		public string IconUri
		{
			get
			{
				return this._iconUri;
			}
			set
			{
				if (this._iconUri != value)
				{
					this._iconUri = value;
					this.OnPropertyChanged();
				}
			}
		}
		private string _iconUri;

		public MastodonAccount(int id)
		{
			this.Id = id;
		}

		public MastodonAccount(Mastonet.Entities.Account account)
		{
			this.Id = account.Id;
			this.Name = account.DisplayName;
			this.IconUri = account.AvatarUrl;
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
