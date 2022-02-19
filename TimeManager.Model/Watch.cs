using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace TimeManager.Model
{
    public class Watch: ICloneable
    {
        static readonly Stopwatch Timer = new();
        internal static bool IsDisplayingDays { get; set; }
        public static string FormatTime(TimeSpan time)
        {
            return IsDisplayingDays ?
                $"{(int)time.TotalDays} {time.Hours:00}:{time.Minutes:00}:{time.Seconds:00}" :
                $"{(int)time.TotalHours:00}:{time.Minutes:00}:{time.Seconds:00}";
        }
        /// <summary>
        /// Update in AppModel.Latancy * 2
        /// </summary>
        internal static event EventHandler<EventArgs> TimerUpdated;
        internal static event EventHandler<EventArgs> TimerStopped;
        internal static void Update()
        {
            do
            {
                TimerUpdated?.Invoke(null, EventArgs.Empty);
                AppModel.CancellationTokenSource.Token.WaitHandle.WaitOne(AppModel.Latancy * 2);
            } while (!AppModel.CancellationTokenSource.Token.IsCancellationRequested);
        }
        internal static void StopTimer(Watch sender)
        {
            Timer.Stop();
            TimerStopped?.Invoke(sender, EventArgs.Empty);
        }


        TimeSpan onStartTime = new();
        TimeSpan previousElapsedWatch = new();
        public bool IsRunning { get; private set; }

        public TimeSpan PreviousElapsedWatch
        {
            get => previousElapsedWatch;
            private set => previousElapsedWatch = value;
        }
        public TimeSpan ElapsedWatch
        {
            get => GetEllapsedTimeWatch();
            set => SetEllapsedTimeWatch(value);
        }


        public Watch()
        {
            TimerStopped += Watch_TimerStopping;
        }
        public Watch(TimeSpan elapsedWatch) : this()
        {
            ElapsedWatch = elapsedWatch;
        }
        ~Watch()
        {
            StopWatch();
            TimerStopped -= Watch_TimerStopping;
        }
        TimeSpan GetEllapsedTimeWatch()
        {
            if (onStartTime == TimeSpan.Zero)
                return PreviousElapsedWatch;
            return PreviousElapsedWatch + Timer.Elapsed - onStartTime;
        }
        public object Clone()
        {
            return new Watch(ElapsedWatch);
        }


        protected void StartWatch()
        {
            if (!Timer.IsRunning)
                Timer.Start();
            onStartTime = Timer.Elapsed;
            IsRunning = true;
        }
        protected void StopWatch()
        {
            if (!IsRunning)
                return;
            IsRunning = false;
            if (onStartTime != TimeSpan.Zero)
            {
                PreviousElapsedWatch += Timer.Elapsed - onStartTime;
                onStartTime = TimeSpan.Zero;
            }
        }
        protected void Watch_TimerStopping(object sender, EventArgs e)
        {
            if (IsRunning)
                StopWatch();
        }
        protected void ResetEllapsedTimeWatch()
        {
            StopWatch();
            SetEllapsedTimeWatch(TimeSpan.Zero);
        }
        protected void SetEllapsedTimeWatch(TimeSpan newTime)
        {
            var previousState = IsRunning;
            onStartTime = TimeSpan.Zero;
            if (previousState)
                StopWatch();
            PreviousElapsedWatch = new TimeSpan(newTime.Ticks);
            if (previousState)
                StartWatch();
        }
        public override string ToString()
        {
            return FormatTime(ElapsedWatch);
        }
    }
}
