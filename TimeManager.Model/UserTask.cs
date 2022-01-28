using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace TimeManager.Model
{
    public class UserTaskEventArgs : EventArgs
    {
        public TimeSpan NewValue { get; private set; }
        public UserTaskEventArgs(TimeSpan newValue) : base()
        {
            NewValue = newValue;
        }
    }
    public class UserTask : ICloneable
    {
        public struct Tag
        {
            public string Name;
        }

        public string Name { get; set; }

        public bool IsSelected { get; set; } = false;
        public bool IsHiddenTask { get; set; }
        public List<Tag> Tags { get; set; }
        public DateTime PreviousSelectionDate { get; set; }
        public DateTime PreviousUnselectionDate { get; set; }
        public DateTime CreationDate { get; set;} = DateTime.Now;

        public TimeData TimeOnTask { get; private set; } = new();

        public event EventHandler<UserTaskEventArgs> UserTaskTimeUpdated;

        public UserTask()
        {
        }

        public UserTask(string name) : this()
        {
            Name = name;
        }

        public object Clone()
        {
            var clone = new UserTask()
            {
                Name = this.Name,
                Tags = this.Tags,
                IsHiddenTask = this.IsHiddenTask,
                PreviousSelectionDate = new DateTime(this.PreviousSelectionDate.Ticks),
                PreviousUnselectionDate = new DateTime(this.PreviousUnselectionDate.Ticks),
                CreationDate = new DateTime(this.CreationDate.Ticks),
                TimeOnTask = new TimeData(new TimeSpan(TimeOnTask.Previous.Ticks), new TimeSpan(TimeOnTask.Current.Ticks)),
            };
            return clone;
        }
        public void Select(Timeline timeline)
        {
            PreviousSelectionDate = DateTime.Now;

            SetTime(timeline);
            IsSelected = true;
        }
        public void Unselect(Timeline timeline)
        {
            IsSelected = false;
            PreviousUnselectionDate = DateTime.Now;
            Save(timeline);
            ResetTime();
        }
        public void SetTime(Timeline timeline)
        {
            TimeOnTask.SetTimerValue(new TimeSpan(timeline.TimeBlocks
                .Where(block => block.Type == TimeBlockTypes.Work)
                .Sum(block => block.Elapsed.Ticks)));
        }
        public void ResetTime()
        {
            TimeOnTask.Reset();
        }

        public UserTask Merge(UserTask task)
        {
            if(task.CreationDate > CreationDate)
            {
                CreationDate = task.CreationDate;
            }
            else
            {
                task.CreationDate = CreationDate;
            }
            Name = task.Name;
            Tags = task.Tags;
            IsHiddenTask = false;
            return task;
        }

        public void Save(Timeline timeline)
        {
            if (!IsSelected)
                return;
            TimeOnTask.SetCurrent(new TimeSpan(timeline.TimeBlocks
                .Where(block => block.Type == TimeBlockTypes.Work)
                .Sum(block => block.Elapsed.Ticks))
            );
            UserTaskTimeUpdated?.Invoke(this, new UserTaskEventArgs(new TimeSpan(TimeOnTask.Value.Ticks)));
        }
        public override string ToString()
        {
            var line = $"{Name}\n{TimeOnTask}, cur. {Watch.FormatTime(TimeOnTask.Current)}";
            return line;
        }

    }
}
