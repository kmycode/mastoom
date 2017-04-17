using Mastonet;
using Mastoom.Shared.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Mastoom.Shared.Models.Mastodon.Status
{
    public class PostStatusModel : INotifyPropertyChanged
    {
		private readonly MastodonClient client;

		/// <summary>
		/// 投稿するテキスト
		/// </summary>
		public string Text
		{
			get
			{
				return this._text;
			}
			set
			{
				if (this._text != value)
				{
					this._text = value;
					this.OnPropertyChanged();

					this.TextLength = this.Text.Length;
					this.TextLengthLeave = 500 - this.TextLength;
				}
			}
		}
		private string _text;

		/// <summary>
		/// テキストの長さ
		/// </summary>
		public int TextLength
		{
			get
			{
				return this._textLength;
			}
			private set
			{
				if (this._textLength != value)
				{
					this._textLength = value;
					this.OnPropertyChanged();
				}
			}
		}
		private int _textLength;

		/// <summary>
		/// 残り文字数
		/// </summary>
		public int TextLengthLeave
		{
			get
			{
				return this._textLengthLeave;
			}
			private set
			{
				if (this._textLengthLeave != value)
				{
					this._textLengthLeave = value;
					this.OnPropertyChanged();
				}
			}
		}
		private int _textLengthLeave;

		/// <summary>
		/// 表示するメッセージ
		/// </summary>
		public string ErrorMessage
		{
			get
			{
				return this._errorMessage;
			}
			private set
			{
				if (this._errorMessage != value)
				{
					this._errorMessage = value;
					this.OnPropertyChanged();
				}
			}
		}
		private string _errorMessage;


		public PostStatusModel(MastodonClient client)
		{
			this.client = client;
		}

		public void Post()
		{
			this.ErrorMessage = "";

			Task.Run(async () =>
			{
				try
				{
					await this.client.PostStatus(this.Text, Visibility.Public);
					GuiThread.Run(() =>
					{
						this.Text = "";
					});
				}
				catch (Exception e)
				{
					GuiThread.Run(() =>
					{
						this.ErrorMessage = "エラーが発生しました";
					});
				}
			});
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
