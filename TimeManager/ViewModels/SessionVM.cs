using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Lab01.Services;

namespace TimeManager.ViewModels
{
    public class SessionVM : INotifyPropertyChanged
	{
		public SessionVM()
		{
			
		}
		#region Input

		#endregion

		#region Output
		private string name = "";
		public string Name
		{
			get => name;
			set
			{
				name = value;
				OnPropertyChanged(nameof(Name));
			}
		}
		private string time = "";
		public string Time
		{
			get => time;
			set
			{
				time = value;
				OnPropertyChanged(nameof(Time));
			}
		}
		private string date = "";
		public string Date
		{
			get => date;
			set
			{
				date = value;
				OnPropertyChanged(nameof(Date));
			}
		}
		#endregion

		#region Commands
		private RelayCommand removeCommand;
		public RelayCommand RemoveCommand
		{
			get
			{
				return removeCommand ??
				  (removeCommand = new RelayCommand(obj =>
				  {

				  }));
			}
		}
		#endregion
		#region INCP
		public event PropertyChangedEventHandler PropertyChanged;
		public void OnPropertyChanged([CallerMemberName]string prop = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
		}
		#endregion
	}
}
