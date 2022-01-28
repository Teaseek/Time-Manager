using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TimeManager.Model
{
	public static class AppModel
	{
		public static readonly CancellationTokenSource CancellationTokenSource = new();
		public static int Latancy => 100;
		public static User User { get; set; } = new();

		public static void StartUpdating()
		{
			if (User.CurrentTask is null)
				return;
			bool isUpdating = true;
			void Update()
			{
				do
				{
					if (CancellationTokenSource.Token.IsCancellationRequested)
					{
						User.Timeline.ResetTimelineQueue();
                        isUpdating = false;
                        break;
					}
					User.Timeline.DoCycle();
					CancellationTokenSource.Token.WaitHandle.WaitOne(Latancy);
				} while (isUpdating);
			}
			var update = new Action(Update);
			Task.Run(update, CancellationTokenSource.Token);
			Task.Run(Watch.Update, CancellationTokenSource.Token);
		}

		public static void StopUpdating()
        {
			CancellationTokenSource.Cancel();
		}


	}
}
