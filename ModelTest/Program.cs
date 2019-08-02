using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeManager.Model;

namespace ModelTest
{
	class Program
	{
		static void Main(string[] args)
		{
			do
			{
				ConsoleKey key = ConsoleKey.NoName;
				SelectSession(ShowMenu());
				AppModel.CurrentTimer.StartTimer();
				do
				{
					key = Console.ReadKey().Key;
					switch (key)
					{
						case ConsoleKey.S:
							AppModel.CurrentTimer.StartTimer();
							break;
						case ConsoleKey.P:
							AppModel.CurrentTimer.PauseTimer();
							break;
						case ConsoleKey.R:
							AppModel.CurrentTimer.ResetTimer();
							break;
						case ConsoleKey.K:
							AppModel.CurrentTimer.Skip();
							break;
						case ConsoleKey.Escape:
							UnselectSession();
							break;
					}
				} while (key != ConsoleKey.Escape);

				ShowMenu();
			} while (true);
		}

		static Session ShowMenu()
		{
			var selectedLine = 0;
			ConsoleKey key = ConsoleKey.NoName;
			do
			{
				Console.WriteLine($"Select sessions:");
				if (AppModel.Sessions.Count > 0)
				{
					for (var index = 0; index < AppModel.Sessions.Count; index++)
					{
						var session = AppModel.Sessions[index];
						if (index == selectedLine)
							Console.WriteLine($">  session {session.Name}");
						else
							Console.WriteLine($"   session {session.Name}");
					}
				}
				if ((AppModel.Sessions.Count == 0 && selectedLine == 0) || AppModel.Sessions.Count == selectedLine)
					Console.WriteLine($">  Create session");
				else
					Console.WriteLine($"   Create session");
				switch (key)
				{
					case ConsoleKey.DownArrow:
						selectedLine++;
						break;
					case ConsoleKey.UpArrow:
						selectedLine--;
						break;
					case ConsoleKey.Enter:
						if (AppModel.Sessions.Count > 0 && selectedLine < AppModel.Sessions.Count)
							return AppModel.Sessions[selectedLine];
						return AppModel.AddSession(new Timer(), "1st");
				}
				if ((AppModel.Sessions.Count == 0 && selectedLine > 0))
					selectedLine = 0;
				if (selectedLine > AppModel.Sessions.Count)
					selectedLine = AppModel.Sessions.Count;
				if (selectedLine < 0)
					selectedLine = 0;
				key = Console.ReadKey().Key;
				Console.Clear();
			} while (key != ConsoleKey.Escape);
			return null;
		}

		static void CreateSession()
		{
			AppModel.AddSession(new Timer(), "1st");
		}
		static void SelectSession(Session session)
		{
			if (session == null)
				return;
			AppModel.SetCurrentSession(session);
			AppModel.CurrentTimer.DefaultFreeTime = 1;
			AppModel.CurrentTimer.DefaultWorkingTime = 1;
			AppModel.CurrentTimer.Tick += CurrentTimer_Tick;
			AppModel.CurrentTimer.PauseTick += CurrentTimer_Tick;

			AppModel.CurrentTimer.TimerStarted += CurrentTimer_TimerStarted;
			AppModel.CurrentTimer.TimerPaused += CurrentTimer_TimerPaused;
			AppModel.CurrentTimer.TimerReseted += CurrentTimer_TimerReseted;
			AppModel.CurrentTimer.TimerSkipped += CurrentTimer_TimerSkipped;

			AppModel.CurrentTimer.WorkingTimeStarted += CurrentTimer_WorkingTimeStarted;
			AppModel.CurrentTimer.WorkingTimeEnded += CurrentTimer_WorkingTimeEnded;

			AppModel.CurrentTimer.FreeTimeStarted += CurrentTimer_FreeTimeStarted;
			AppModel.CurrentTimer.FreeTimeEnded += CurrentTimer_FreeTimeEnded;
		}
		static void UnselectSession()
		{
			AppModel.CurrentTimer.Tick -= CurrentTimer_Tick;
			AppModel.CurrentTimer.PauseTick -= CurrentTimer_Tick;

			AppModel.CurrentTimer.TimerStarted -= CurrentTimer_TimerStarted;
			AppModel.CurrentTimer.TimerPaused -= CurrentTimer_TimerPaused;
			AppModel.CurrentTimer.TimerReseted -= CurrentTimer_TimerReseted;
			AppModel.CurrentTimer.TimerSkipped -= CurrentTimer_TimerSkipped;

			AppModel.CurrentTimer.WorkingTimeStarted -= CurrentTimer_WorkingTimeStarted;
			AppModel.CurrentTimer.WorkingTimeEnded -= CurrentTimer_WorkingTimeEnded;

			AppModel.CurrentTimer.FreeTimeStarted -= CurrentTimer_FreeTimeStarted;
			AppModel.CurrentTimer.FreeTimeEnded -= CurrentTimer_FreeTimeEnded;
		}
		#region Events

		#region Timer

		private static void CurrentTimer_TimerSkipped(Timer e, string Message)
		{
			Console.WriteLine($"---Timer Skipped");
		}
		private static void CurrentTimer_TimerReseted(Timer e, string Message)
		{
			Console.WriteLine($"---Timer Reseted");
		}
		private static void CurrentTimer_TimerPaused(Timer e, string Message)
		{
			Console.WriteLine($"---Timer Paused");
		}
		private static void CurrentTimer_TimerStarted(Timer e, string Message)
		{
			Console.WriteLine($"---Timer Started;");
			Console.WriteLine($"-------date: {e.CurrentDate}");
			Console.WriteLine($"-------DefaultFreeTime: {e.DefaultFreeTime}");
			Console.WriteLine($"-------DefaultWorkingTime: {e.DefaultWorkingTime}");
		}

		#endregion

		#region onWork

		private static void CurrentTimer_WorkingTimeStarted(Timer e, string Message)
		{
			Console.WriteLine($"---Working Time Started;");
		}
		private static void CurrentTimer_WorkingTimeEnded(Timer e, string Message)
		{
			Console.WriteLine($"---Working Time Ended;");
		}

		#endregion

		#region onFree

		private static void CurrentTimer_FreeTimeStarted(Timer e, string Message)
		{
			Console.WriteLine($"---Free Time Started;");
		}
		private static void CurrentTimer_FreeTimeEnded(Timer e, string Message)
		{
			Console.WriteLine($"---Free Time Ended;");
		}

		#endregion

		private static void CurrentTimer_Tick(Timer e, string Message)
		{
			Console.Clear();
			Console.WriteLine($"{e}");
		}

		#endregion
	}
}
