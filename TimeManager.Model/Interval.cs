using System;

namespace TimeManager.Model
{
    public enum CreationCauses
    {
        TaskChange,
        AutoSkip,
        ForceSkip,
        Start,
        Stop,
        Pause,
        Reset,
        Close,
        Empty = -1
    }
    public class Interval
    {
        public Interval()
        {
        }

        public Interval(DateTime dateStart, DateTime dateEnd, CreationCauses creationCause) : this()
        {
            DateStart = dateStart;
            DateEnd = dateEnd;
            CreationCause = creationCause;
        }

        public Interval(UserTask task, TimeBlock timeBlock, CreationCauses creationCause): this(DateTime.Now, DateTime.Now, creationCause)
        {
            TaskOnStart = Task = (UserTask)task.Clone();
            TimeBlockOnStart = TimeBlock = (TimeBlock)timeBlock.Clone();
        }

        public DateTime DateStart { get; set; } = DateTime.MinValue;
        public DateTime DateEnd { get; set; } = DateTime.MinValue;

        public CreationCauses CreationCause { get; set; } = CreationCauses.Empty;

        public UserTask TaskOnStart { get; set; }
        public TimeBlock TimeBlockOnStart { get; set; }
        public UserTask Task { get; set; }
        public TimeBlock TimeBlock { get; set; }

        public TimeSpan GetInterval()
        {
            if (DateStart != DateTime.MinValue && DateEnd != DateTime.MinValue)
                return DateEnd - DateStart;
            return TimeSpan.Zero;
        }
    }
}