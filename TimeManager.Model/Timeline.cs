using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeManager.Model
{
    public class TimeBlockCycleEventArgs : EventArgs
    {
        public CreationCauses Reason { get; private set; }
        public TimeBlock ActiveTimeBlock { get; private set; }
        public TimeBlockCycleEventArgs(TimeBlock activeTimeBlock, CreationCauses reason) : base()
        {
            ActiveTimeBlock = activeTimeBlock;
            Reason = reason;
        }
    }
    public class Timeline
    {
        public LinkedList<TimeBlock> TimeBlocks { get; set; } = new();
        public TimeBlock ActiveTimeBlock => GetActiveTimeBlock();
        public LinkedListNode<TimeBlock> CurrentTimeBlock { get; set; }
        public Dictionary<TimeBlockTypes, TimeBlock> ServiceTimeBlocks { get; private set; } = new();
        public Action<Timeline>[] SaveTime { get; set; }
        public bool IsPaused { get; private set; } = false;
        public bool IsStopped { get; private set; } = true;
        public bool IsRunning { get; private set; } = false;

        public event EventHandler<TimeBlockCycleEventArgs> TimeBlockCycle;


        public Timeline()
        {
            var tags = new TimeBlock.TimeBlockTag[] {
                new TimeBlock.TimeBlockTag() { Name = "new" },
                new TimeBlock.TimeBlockTag() { Name = "old" }
            };
            AddTimeBlock("work", TimeBlockTypes.Work, tags[0], TimeSpan.FromSeconds(20));
            AddTimeBlock("free", TimeBlockTypes.Rest, TimeSpan.FromSeconds(5));
            AddTimeBlock("work", TimeBlockTypes.Work, tags[1], TimeSpan.FromSeconds(20));
            AddTimeBlock("free", TimeBlockTypes.Rest, TimeSpan.FromSeconds(5));
            AddTimeBlock("work", TimeBlockTypes.Work, tags[1], TimeSpan.FromSeconds(20));
            AddTimeBlock("long free", TimeBlockTypes.Rest, TimeSpan.FromSeconds(30));
            Watch.TimerUpdated += Watch_TimerUpdated;
            ServiceTimeBlocks.Add(TimeBlockTypes.Pause, new TimeBlock(TimeBlockTypes.Pause));
            ServiceTimeBlocks.Add(TimeBlockTypes.Stop, new TimeBlock(TimeBlockTypes.Stop));
        }
        ~Timeline()
        {
            Watch.TimerUpdated -= Watch_TimerUpdated;
        }
        public void AddTimeBlock(string name, TimeBlockTypes type, TimeSpan endTime)
        {
            TimeBlocks.AddLast(new LinkedListNode<TimeBlock>(new TimeBlock(name, type, endTime)));
            if (CurrentTimeBlock == null)
                CurrentTimeBlock = TimeBlocks.First;
        }
        public void AddTimeBlock(string name, TimeBlockTypes type, TimeBlock.TimeBlockTag tag, TimeSpan endTime)
        {
            TimeBlocks.AddLast(new LinkedListNode<TimeBlock>(new TimeBlock(name, type, tag, endTime)));
            if (CurrentTimeBlock == null)
                CurrentTimeBlock = TimeBlocks.First;
        }

        private TimeBlock GetActiveTimeBlock()
        {
            if (IsStopped)
                return ServiceTimeBlocks[TimeBlockTypes.Stop];
            if (IsPaused)
                return ServiceTimeBlocks[TimeBlockTypes.Pause];
            return CurrentTimeBlock.Value;
        }
        public void StartTimeline()

        {
            UpdateStat();
            IsStopped = false;
            IsPaused = false;
            IsRunning = true;
            if (!CurrentTimeBlock.Value.IsRunning)
                CurrentTimeBlock.Value.Start();
            TimeBlockCycle?.Invoke(this, new TimeBlockCycleEventArgs(ActiveTimeBlock, CreationCauses.Start));
        }
        public void StopTimeline(bool IsReset = false)
        {
            UpdateStat();
            IsRunning = false;
            CurrentTimeBlock.Value.Stop();
            if (IsPaused)
                IsPaused = false;
            IsStopped = true;
            TimeBlockCycle?.Invoke(this, new TimeBlockCycleEventArgs(ActiveTimeBlock, IsReset ? CreationCauses.Reset : CreationCauses.Stop));
        }
        public void PauseTimeline(bool isTaskChange = false)
        {
            UpdateStat();
            IsRunning = false;
            CurrentTimeBlock.Value.Pause();
            if (IsStopped)
                return;
            IsPaused = true;
            TimeBlockCycle?.Invoke(this, new TimeBlockCycleEventArgs(ActiveTimeBlock, isTaskChange ? CreationCauses.TaskChange : CreationCauses.Pause));
        }
        public void SkipTimeBlock(bool isForseSkip = true)
        {
            var previousWatchValue = CurrentTimeBlock.Value.IsRunning;
            CurrentTimeBlock.Value.DoCycle();
            CurrentTimeBlock = CurrentTimeBlock.Next ?? TimeBlocks.First;
            TimeBlockCycle?.Invoke(this, new TimeBlockCycleEventArgs(ActiveTimeBlock, isForseSkip ? CreationCauses.ForceSkip : CreationCauses.AutoSkip));
            if (previousWatchValue)
                CurrentTimeBlock.Value.Start();
        }
        public void ResetTimelineQueue()
        {
            StopTimeline(true);
            foreach (var block in TimeBlocks)
            {
                block.Reset();
            }
            CurrentTimeBlock = TimeBlocks.First;
        }
        public void DoCycle()
        {
            if (CurrentTimeBlock == null)
                CurrentTimeBlock = TimeBlocks.First;

            if (TimeBlocks.Count == 0)
                throw new Exception("Timeblocks is empty");

            if (IsPaused)
            {
                ServiceTimeBlocks[TimeBlockTypes.Pause].Start();
                return;
            }
            else
            {
                ServiceTimeBlocks[TimeBlockTypes.Pause].Stop();
            }
            if (IsStopped)
            {
                ServiceTimeBlocks[TimeBlockTypes.Stop].Start();
                return;
            }
            else
            {
                ServiceTimeBlocks[TimeBlockTypes.Stop].Stop();
            }

            if (CurrentTimeBlock.Value.ElapsedWatch.CompareTo(CurrentTimeBlock.Value.SkipTime) == 1)
            {
                UpdateStat();
                SkipTimeBlock(false);
            }
        }
        private void Watch_TimerUpdated(object sender, EventArgs e) => UpdateStat();
        void UpdateStat()
        {
            foreach (var save in SaveTime)
            {
                save?.Invoke(this);
            }
        }

        public override string ToString()
        {
            var serviceLine = $"";
            foreach (var block in ServiceTimeBlocks)
            {
                serviceLine += $"\t{(block.Value.IsRunning ? ">" : " ")} {block.Value}\n";
            }
            return $"active block[{ActiveTimeBlock.Name}]:" +
                $"\n\t{(CurrentTimeBlock.Value.IsRunning ? ">" : " ")} {CurrentTimeBlock?.Value}" +
                $"\n{serviceLine}";
        }
    }
}
