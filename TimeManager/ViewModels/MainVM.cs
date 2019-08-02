using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Lab01.Services;

namespace TimeManager.ViewModels
{
    public class MainVM : INotifyPropertyChanged
	{
		public MainVM()
		{
			
		}

		#region Input
		private string currentSession = "";
		public string CurrentSession
		{
			get => currentSession;
			set
			{
				currentSession = value;
				OnPropertyChanged(nameof(CurrentSession));
			}
		}
		private string currentTime = "";
		public string CurrentTime
		{
			get => currentTime;
			set
			{
				currentTime = value;
				OnPropertyChanged(nameof(CurrentTime));
			}
		}
		private string currentDate = "";
		public string CurrentDate
		{
			get => currentDate;
			set
			{
				currentDate = value;
				OnPropertyChanged(nameof(CurrentDate));
			}
		}

		private string onWorkNotificationText= "";
		public string OnWorkNotificationText
		{
			get => onWorkNotificationText;
			set
			{
				onWorkNotificationText = value;
				OnPropertyChanged(nameof(OnWorkNotificationText));
			}
		}
		private string onFreeNotificationText = "";
		public string OnFreeNotificationText
		{
			get => onFreeNotificationText;
			set
			{
				onFreeNotificationText = value;
				OnPropertyChanged(nameof(OnFreeNotificationText));
			}
		}


		private string workingTime = "";
		public string WorkingTime
		{
			get => workingTime;
			set
			{
				workingTime = value;
				OnPropertyChanged(nameof(WorkingTime));
			}
		}
		private string freeTime = "";
		public string FreeTime
		{
			get => freeTime;
			set
			{
				freeTime = value;
				OnPropertyChanged(nameof(FreeTime));
			}
		}

		private bool? isSideNoticesDisplayed = true;
		public bool? IsSideNoticesDisplayed
		{
			get => isSideNoticesDisplayed;
			set
			{
				isSideNoticesDisplayed = value;
				OnPropertyChanged(nameof(IsSideNoticesDisplayed));
			}
		}
		private bool? isCenterNoticesDisplayed = true;
		public bool? IsCenterNoticesDisplayed
		{
			get => isCenterNoticesDisplayed;
			set
			{
				isCenterNoticesDisplayed = value;
				OnPropertyChanged(nameof(IsCenterNoticesDisplayed));
			}
		}
		private bool? isTimeRemainingDisplayed = true;
		public bool? IsTimeRemainingDisplayed
		{
			get => isTimeRemainingDisplayed;
			set
			{
				isTimeRemainingDisplayed = value;
				OnPropertyChanged(nameof(IsTimeRemainingDisplayed));
			}
		}
		#endregion

		#region Output

		private ObservableCollection<SessionVM> sessions = new ObservableCollection<SessionVM>();
		public ObservableCollection<SessionVM> Sessions
		{
			get => sessions;
			set
			{
				sessions = value;
				OnPropertyChanged(nameof(Sessions));
			}
		}
		private SessionVM session = null;
		public SessionVM Session
		{
			get => session;
			set
			{
				session = value;
				OnPropertyChanged(nameof(Session));
			}
		}
		private string elapsedTime = "";
		public string ElapsedTime
		{
			get => elapsedTime;
			set
			{
				elapsedTime = value;
				OnPropertyChanged(nameof(ElapsedTime));
			}
		}
		private string currentWorkingTime = "";
		public string CurrentWorkingTime
		{
			get => currentWorkingTime;
			set
			{
				currentWorkingTime = value;
				OnPropertyChanged(nameof(CurrentWorkingTime));
			}
		}
		private string currentFreeTime = "";
		public string CurrentFreeTime
		{
			get => currentFreeTime;
			set
			{
				currentFreeTime = value;
				OnPropertyChanged(nameof(currentFreeTime));
			}
		}
		#endregion

		#region Commands
		private RelayCommand startCommand;
		public RelayCommand StartCommand
		{
			get
			{
				return startCommand ??
				  (startCommand = new RelayCommand(obj =>
				  {

				  }));
			}
		}
		private RelayCommand resetCommand;
		public RelayCommand ResetCommand
		{
			get
			{
				return resetCommand ??
				       (resetCommand = new RelayCommand(obj =>
				       {

				       }));
			}
		}

		private RelayCommand saveCommand;
		public RelayCommand SaveCommand
		{
			get
			{
				return saveCommand ??
				       (saveCommand = new RelayCommand(obj =>
				       {

				       }));
			}
		}

		private RelayCommand resetSettingsCommand;
		public RelayCommand ResetSettingsCommand
		{
			get
			{
				return resetSettingsCommand ??
				       (resetSettingsCommand = new RelayCommand(obj =>
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
