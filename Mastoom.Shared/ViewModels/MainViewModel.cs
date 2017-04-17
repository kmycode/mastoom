using Mastoom.Shared.Models.Mastodon.Connection;
using Mastoom.Shared.Models.Mastodon.Status;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mastoom.Shared.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
		#region プロパティ

		/// <summary>
		/// 接続先一覧
		/// </summary>
		public MastodonConnectionCollection Connections { get; } = new MastodonConnectionCollection();

		/// <summary>
		/// メインとなる接続先のステータス一覧
		/// </summary>
		public MastodonStatusCollection MainStatuses => this.Connections.Main?.Statuses;

		/// <summary>
		/// ステータス投稿のモデル
		/// </summary>
		public PostStatusModel PostStatus => this.Connections.Main?.PostStatus;

		#endregion

		#region メソッド

		public MainViewModel()
		{
			this.Connections.PropertyChanged += this.RaisePropertyChanged;

			this.Connections.AddTestConnection();

			this.Connections.Main.PropertyChanged += this.RaisePropertyChanged;
		}

		#endregion

		#region コマンド


		public RelayCommand<PostStatusModel> PostStatusCommand
		{
			get
			{
				return this._postStatusCommand = this._postStatusCommand ?? new RelayCommand<PostStatusModel>((post) =>
				{
					post.Post();
				});
			}
		}
		private RelayCommand<PostStatusModel> _postStatusCommand;


		#endregion
	}
}
