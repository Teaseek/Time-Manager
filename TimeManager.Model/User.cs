using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeManager.Model
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public List<UserTask> Tasks { get; } = new();
        public UserTask HiddenTask { get; private set; }
        public UserTask CurrentTask { get; private set; }
        public UserStatistics UserStatistics { get; private set; } = new();
        public Timeline Timeline { get; private set; } = new();

        public User()
        {
            Timeline.TimeBlockCycle += TimeBlockCycle;
            SelectHidden();
        }
        ~User()
        {
            Timeline.TimeBlockCycle -= TimeBlockCycle;
        }
        public void ResetUser()
        {
            UnselectTask();
            Timeline.ResetTimelineQueue();
        }

        private void TimeBlockCycle(object sender, TimeBlockCycleEventArgs e)
        {
            //CurrentTask.ResetTime();
            if (CurrentTask == null)
            {
                SelectHidden();
            }
            UserStatistics.Add(CurrentTask, e.ActiveTimeBlock, e.Reason);
            //CurrentTask.SetTime(Timeline);
        }

        public UserTask AddTask(string name)
        {
            var task = new UserTask(name);
            Tasks.Add(task);
            return task;
        }
        public void UnselectTask(bool isSelectHiddenTask = false)
        {
            if (CurrentTask == null)
                return;
            if (CurrentTask.IsHiddenTask)
                return;
            if (isSelectHiddenTask)
            {
                SelectHidden();
            }
            else
            {
                Timeline.PauseTimeline(true);
                CurrentTask.Unselect(Timeline);
                CurrentTask.UserTaskTimeUpdated -= UserTaskTimeUpdated;
            }
        }
        public void SelectHidden()
        {
            HiddenTask = new("Temp task") { IsHiddenTask = true };
            SelectTask(HiddenTask);
        }

        public void SelectTask(UserTask task)
        {
            if (!Tasks.Contains(task) && !task.IsHiddenTask)
                Tasks.Add(task);
            if (CurrentTask != null)
            {
                if (CurrentTask.IsHiddenTask)
                {
                    UserStatistics.MergeHiddenTask(task, CurrentTask);
                }
                UnselectTask();
            }
            CurrentTask = task;
            CurrentTask.Select(Timeline);
            UserStatistics.Add(CurrentTask, Timeline.ActiveTimeBlock, CreationCauses.TaskChange);
            CurrentTask.UserTaskTimeUpdated += UserTaskTimeUpdated;
            Timeline.SaveTime = new Action<Timeline>[2]
            {
                task.Save,
                UserStatistics.Save
            };
        }

        private void UserTaskTimeUpdated(object sender, UserTaskEventArgs e)
        {
            UserStatistics.Save((UserTask)sender);
        }
    }
}
