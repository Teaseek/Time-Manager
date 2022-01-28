using System;
using System.Collections.Generic;
using System.Linq;

namespace TimeManager.Model
{
    public class UserStatistics
    {
        public List<Interval> Intervals { get; private set; } = new();
        public Interval ActiveInterval => Intervals.LastOrDefault();


        public void Add(UserTask currentTask, TimeBlock activeTimeBlock, CreationCauses creationCause)
        {
            if (ActiveInterval != null)
                ActiveInterval.DateEnd = DateTime.Now;
            Interval interval = new(currentTask, activeTimeBlock, creationCause);
            Intervals.Add(interval);
        }

        public void Save(Timeline timeline)
        {
            if (ActiveInterval == null)
                return;
            ActiveInterval.DateEnd = DateTime.Now;
            ActiveInterval.TimeBlock = (TimeBlock)timeline.ActiveTimeBlock.Clone();
        }
        public void Save(UserTask currentTask)
        {
            if (ActiveInterval == null)
                return;
            ActiveInterval.DateEnd = DateTime.Now;
            ActiveInterval.Task = (UserTask)currentTask.Clone();
        }
        public UserTask MergeHiddenTask(UserTask newTask, UserTask hiddenTask)
        {
            newTask.TimeOnTask.Previous += hiddenTask.TimeOnTask.Value;
            for (int i = Intervals.Count - 1; i >= 0; i--)
            {
                if (!Intervals[i].Task.IsHiddenTask)
                    break;
                Intervals[i].Task = (UserTask)Intervals[i].Task.Merge(newTask).Clone();
            }
            return newTask;
        }

        public override string ToString()
        {
            var line = $"stat:\n";
            foreach (var interval in Intervals)
            {
                line += $"[{Watch.FormatTime(interval.DateEnd - interval.DateStart)}] " +
                $"task:{interval.Task.Name} [{Watch.FormatTime(interval.Task.TimeOnTask.Value - interval.TaskOnStart.TimeOnTask.Value)}] " +
                $"type:{Enum.GetName(interval.TimeBlock.Type.GetType(), interval.TimeBlock.Type)} tag:{interval.TimeBlock.Tag?.Name} " +
                $"diff val. {Watch.FormatTime(interval.TimeBlock.ElapsedWatch - interval.TimeBlockOnStart.ElapsedWatch)} {Enum.GetName(interval.CreationCause.GetType(), interval.CreationCause)}\n";
            }
            return line;
        }
    }
}
