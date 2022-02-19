using System;
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace TimeManager.Model
{
    public enum TimeBlockTypes
    {
        Work,
        Rest,
        Pause,
        Stop,
        Other = -1
    }

    public class TimeBlock : Watch, ICloneable
    {
        public class TimeBlockTag : IComparable<TimeBlockTag>
        {
            public string Name { get; set; }
            public int CompareTo(TimeBlockTag other) => Name.CompareTo(other.Name);
        }
        public string Name { get; set; }
        public TimeBlockTypes Type { get; set; }
        public TimeBlockTag Tag { get; set; }
        public TimeSpan SkipTime { get; set; } = TimeSpan.Zero;
        public TimeSpan PreviousElapsed = TimeSpan.Zero;
        public TimeSpan Elapsed
        {
            get => PreviousElapsed + ElapsedWatch;
            set
            {
                ResetEllapsedTimeWatch();
                PreviousElapsed = value;
            }
        }

        public TimeBlock() : base()
        {

        }
        public TimeBlock(string name, TimeBlockTypes type, TimeSpan endTime) : this()
        {
            Name = name;
            Type = type;
            SkipTime = endTime;
        }
        public TimeBlock(string name, TimeBlockTypes type, TimeBlockTag tag, TimeSpan endTime) : this(name, type, endTime)
        {
            Tag = tag;
        }
        public TimeBlock(TimeBlockTypes type) : this()
        {
            Name = Enum.GetName(type.GetType(), type);
            Type = type;
        }


        new public object Clone()
        {
            var clone = new TimeBlock()
            {
                Name = this.Name,
                Type = this.Type,
                Tag = new TimeBlockTag() { Name = this.Tag?.Name },
                SkipTime = new TimeSpan(this.SkipTime.Ticks),
                PreviousElapsed = new TimeSpan(this.PreviousElapsed.Ticks)
            };
            clone.ElapsedWatch = ElapsedWatch;
            return clone;
        }
        public void Start()
        {
            if (!IsRunning)
                StartWatch();
        }
        public void Pause()
        {
            if (IsRunning)
                StopWatch();
        }
        public void Stop()
        {
            if (IsRunning)
                DoCycle();
        }
        public void Reset()
        {
            ResetEllapsedTimeWatch();
            PreviousElapsed = TimeSpan.Zero;
        }
        public void DoCycle()
        {
            if (IsRunning)
                StopWatch();
            PreviousElapsed += ElapsedWatch;
            ResetEllapsedTimeWatch();
        }


        public override string ToString()
        {
            return $"{Name} {FormatTime(ElapsedWatch)}/{SkipTime}  elap. ({FormatTime(Elapsed)})";
        }
    }
}
