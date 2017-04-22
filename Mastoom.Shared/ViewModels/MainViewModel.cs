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
		/// 接続グループ一覧
		/// </summary>
		public MastodonConnectionGroupCollection Groups { get; } = new MastodonConnectionGroupCollection();

		#endregion

		#region メソッド

		public MainViewModel()
		{
			this.Groups.AddTestConnection();
		}

		#endregion

		#region Commands

        /// <summary>
        /// Post new status (toot)
        /// </summary>
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

        /// <summary>
        /// Exit page mode
        /// </summary>
        public RelayCommand<MastodonStatusCollection> ExitPageModeCommand
        {
            get
            {
                return this._exitPageModeCommand = this._exitPageModeCommand ?? new RelayCommand<MastodonStatusCollection>((collection) =>
                {
                    collection.ExitPageMode();
                });
            }
        }
        private RelayCommand<MastodonStatusCollection> _exitPageModeCommand;


        #endregion
    }
}
