using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;

namespace Mastoom.Shared.ViewModels
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {

		public class RelayCommand : ICommand
		{
			private Action action;

			public RelayCommand(Action action)
			{
				this.action = action;
			}

			public event EventHandler CanExecuteChanged;

			public bool CanExecute(object parameter) => true;

			public void Execute(object parameter)
			{
				this.action.Invoke();
			}
		}

		public class RelayCommand<T> : ICommand
		{
			private Action<T> action;

			public RelayCommand(Action<T> action)
			{
				this.action = action;
			}

			public event EventHandler CanExecuteChanged;

			public bool CanExecute(object parameter) => true;

			public void Execute(object parameter)
			{
				if (parameter is T obj)
				{
					this.action.Invoke(obj);
				}
			}
		}

		#region INotifyPropertyChanged

		public event PropertyChangedEventHandler PropertyChanged;
		protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
		protected void RaisePropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(e.PropertyName));
		}

		#endregion

	}
}
