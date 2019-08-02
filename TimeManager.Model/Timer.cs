using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TimeManager.Model
{
	public class Timer
	{
		public Guid id = new Guid();

		public delegate void TimerStateHandler(Timer e, string Message);
		public event TimerStateHandler TimerStarted;
		public event TimerStateHandler TimerPaused;
		public event TimerStateHandler TimerReseted;
		public event TimerStateHandler TimerSkipped;
		public event TimerStateHandler WorkingTimeStarted;
		public event TimerStateHandler WorkingTimeEnded;
		public event TimerStateHandler FreeTimeStarted;
		public event TimerStateHandler FreeTimeEnded;
		public event TimerStateHandler Tick;
		public event TimerStateHandler PauseTick;

		public DateTime CurrentDate { get; set; }

		public string WorkingTime
		{
			get => $"{(int)onWorkingTimeTimer.Elapsed.TotalMinutes:00}:" +
						$"{onWorkingTimeTimer.Elapsed.Seconds:00}";
			set
			{
				if (TimeSpan.TryParse(value, out var time))
				{
					onWorkingTimeTimer.Elapsed.Add(time);
				}
			}
		}
		public string FreeTime
		{
			get => $"{(int)onFreeTimeTimer.Elapsed.TotalMinutes:00}:" +
						$"{onFreeTimeTimer.Elapsed.Seconds:00}";
			set
			{
				if (TimeSpan.TryParse(value, out var time))
				{
					onFreeTimeTimer.Elapsed.Add(time);
				}
			}
		}

		public string ElapsedTime
		{
			get =>
				$"{(int)onElapsedTimeTimer.Elapsed.TotalHours:00}:" +
					 $"{onElapsedTimeTimer.Elapsed.Minutes:00}:" +
					 $"{onElapsedTimeTimer.Elapsed.Seconds:00}";
			set
			{
				if (TimeSpan.TryParse(value, out var time))
				{
					onElapsedTimeTimer.Elapsed.Add(time);
				}
			}
		}
		public string PauseTime
		{
			get =>
				$"{(int)onPauseTimeTimer.Elapsed.TotalHours:00}:" +
				$"{onPauseTimeTimer.Elapsed.Minutes:00}:" +
				$"{onPauseTimeTimer.Elapsed.Seconds:00}";
		}
		public int DefaultFreeTime { get; set; } = 15;
		public int DefaultWorkingTime { get; set; } = 45;

		public int WorkingTimeCycleCount { get; set; }
		public int FreeTimeCycleCount { get; set; }

		public int SkipWorkingTimeCount { get; set; }
		public int SkipFreeTimeCount { get; set; }

		public bool IsShowMinutesLeft { get; set; } = false;

		private TimeSpan FreeTimeSpan => new TimeSpan(0, 0, DefaultFreeTime, 0);
		private TimeSpan WorkingTimeSpan => new TimeSpan(0, 0, DefaultWorkingTime, 0);

		private Stopwatch onElapsedTimeTimer = new Stopwatch();
		private Stopwatch onPauseTimeTimer = new Stopwatch();
		private Stopwatch onWorkingTimeTimer = new Stopwatch();
		private Stopwatch onFreeTimeTimer = new Stopwatch();

		private bool isTimerStarted = false;
		private bool isTimerRunning = false;
		private bool isWorkingTime = true;
		private bool isFreeTime => !isWorkingTime;

		private double TickInSecond = 1;

		private Task cycleAction;
		public Timer()
		{
			cycleAction = new Task(Cycle);
		}

		~Timer()
		{
			isTimerRunning = false;
		}
		public void StartTimer()
		{
			if (cycleAction.IsCanceled)
			{
				cycleAction = new Task(Cycle);
			}
			if (cycleAction.Status != TaskStatus.Running)
			{
				cycleAction.Start();
			}
			if (!isTimerStarted)
			{
				isTimerStarted = true;
			}
			isTimerRunning = true;
		}
		public void PauseTimer()
		{
			isTimerRunning = false;
		}
		public void ResetTimer()
		{
			isTimerRunning = false;
			isWorkingTime = true;
			WorkingTimeCycleCount = 0;
			FreeTimeCycleCount = 0;
			SkipFreeTimeCount = 0;
			SkipWorkingTimeCount = 0;
			onWorkingTimeTimer.Reset();
			onFreeTimeTimer.Reset();
			TimerReseted?.Invoke(this, "");
		}
		private void Reset()
		{
			isTimerStarted = false;
			onElapsedTimeTimer.Reset();
			ResetTimer();
		}
		public void Skip()
		{
			if (!isTimerRunning || !isTimerStarted)
				return;
			if (isWorkingTime)
			{
				SkipWorkingTimeCount++;
				isWorkingTime = false;
				onWorkingTimeTimer.Reset();
			}
			else if (isFreeTime)
			{
				SkipFreeTimeCount++;
				isWorkingTime = true;
				onFreeTimeTimer.Reset();
			}
			else
			{
				return;
			}
			TimerSkipped?.Invoke(this, "");
		}
		private void Cycle()
		{
			var tickTimer = new Stopwatch();
			do
			{
				if (isTimerStarted && !isTimerRunning)
				{
					if (!onPauseTimeTimer.IsRunning)
						onPauseTimeTimer.Start();
					if (!tickTimer.IsRunning)
					{
						tickTimer.Start();
						PauseTick?.Invoke(this, "");
					}
					else if (tickTimer.Elapsed.TotalSeconds >= TickInSecond) //fix
					{
						PauseTick?.Invoke(this, "");
						tickTimer.Restart();
					}
					continue;
				}

				if (isTimerStarted && isTimerRunning && onPauseTimeTimer.IsRunning)
				{
					onPauseTimeTimer.Stop();
				}
				if (!onElapsedTimeTimer.IsRunning)
				{
					onElapsedTimeTimer.Start();
				}
				TimerStarted?.Invoke(this, "");
				while (isTimerRunning)
				{
					if (!isTimerStarted)
						break;
					if (isTimerRunning && onPauseTimeTimer.IsRunning)
					{
						onPauseTimeTimer.Stop();
					}
					CurrentDate = DateTime.Today;
					if (isWorkingTime)
					{
						if (onFreeTimeTimer.IsRunning)
							onFreeTimeTimer.Stop();
						if (!onWorkingTimeTimer.IsRunning)
							onWorkingTimeTimer.Start();
						if (!onElapsedTimeTimer.IsRunning)
							onElapsedTimeTimer.Start();

						if (onWorkingTimeTimer.Elapsed.CompareTo(WorkingTimeSpan) == 1)
						{
							WorkingTimeCycleCount++;
							onWorkingTimeTimer.Reset();
							onElapsedTimeTimer.Stop();
							isWorkingTime = false;
							WorkingTimeEnded?.Invoke(this, $"");
							FreeTimeStarted?.Invoke(this, "");
						}
					}
					else if (isFreeTime)
					{
						if (onWorkingTimeTimer.IsRunning)
							onWorkingTimeTimer.Stop();
						if (onElapsedTimeTimer.IsRunning)
							onElapsedTimeTimer.Stop();
						if (!onFreeTimeTimer.IsRunning)
							onFreeTimeTimer.Start();

						if (onFreeTimeTimer.Elapsed.CompareTo(FreeTimeSpan) == 1)
						{
							FreeTimeCycleCount++;
							onFreeTimeTimer.Reset();
							isWorkingTime = true;
							onElapsedTimeTimer.Start();
							FreeTimeEnded?.Invoke(this, "");
							WorkingTimeStarted?.Invoke(this, "");
						}
					}

					if (!tickTimer.IsRunning)
					{
						tickTimer.Start();
						Tick?.Invoke(this, "");
					}
					else if (tickTimer.Elapsed.TotalSeconds >= TickInSecond) //fix
					{
						Tick?.Invoke(this, "");
						tickTimer.Restart();
					}
				}
				onElapsedTimeTimer.Stop();
				onWorkingTimeTimer.Stop();
				onFreeTimeTimer.Stop();
				TimerPaused?.Invoke(this, "");
			} while (isTimerStarted);
		}

		public override string ToString()
		{
			var started = isTimerStarted ? "Started" : "Not started";
			var running = isTimerRunning ? "Running" : "Not running";
			var working = isWorkingTime ? ">" : " ";
			var free = isFreeTime ? ">" : " ";
			return $"Timer {started} and {running}:" +
				   $"\n\t{working}Work:{WorkingTime} [{WorkingTimeCycleCount}] skipped:{SkipWorkingTimeCount};" +
				   $"\n\t{free}Free:{FreeTime} [{FreeTimeCycleCount}] skipped:{SkipFreeTimeCount};" +
				   $"\n\tElap.:{ElapsedTime}." +
				   $"\n\tPause:{PauseTime}.";
		}
	}
}
