using System;
using System.Linq;
using ConsoleTools;
using TimeManager.Model;

namespace ModelTest
{
    class Program
    {
        static void Main(string[] args)
        {
            AppModel.User.AddTask("some");
            AppModel.User.AddTask("some 2");
            AppModel.User.AddTask("some 3");
            AppModel.User.AddTask("some 4");
            AppModel.User.AddTask("some 5");
            AppModel.User.AddTask("some 6");
            AppModel.StartUpdating();


            var menu = new ConsoleMenu(args, level: 1)
                .Add("Show tasks", () => ShowTasks(args).Show())
                .Add("Unselect task", () => AppModel.User.UnselectTask(true))
                .Add("Show statistics", () => ShowStat(args).Show())
                .Add("Start timeline", () => AppModel.User.Timeline.StartTimeline())
                .Add("Pause timeline", () => AppModel.User.Timeline.PauseTimeline())
                .Add("Stop timeline", () => AppModel.User.Timeline.StopTimeline())
                .Add("Reset timeline", () => AppModel.User.Timeline.ResetTimelineQueue())
                .Add("Skip block", () => AppModel.User.Timeline.SkipTimeBlock())
                .Add("Close", ConsoleMenu.Close)
                .Configure(config =>
                {
                    config.Selector = "--> ";
                    config.EnableWriteTitle = true;
                    config.WriteTitleAction = title => Console.WriteLine($"{title}\n{AppModel.User.CurrentTask}\n{AppModel.User.Timeline}");
                });
            menu.Show();
        }
        public static ConsoleMenu ShowStat(string[] args)
        {
            var subMenu = new ConsoleMenu(args, level: 2);

            return subMenu.Add("Back", ConsoleMenu.Close)
                .Configure(config =>
                {
                    config.Selector = "--> ";
                    config.EnableWriteTitle = true;
                    config.WriteTitleAction = title => Console.WriteLine(AppModel.User.UserStatistics.ToString());
                });
        }
        public static ConsoleMenu ShowTasks(string[] args)
        {
            var subMenu = new ConsoleMenu(args, level: 2);
            foreach (var task in AppModel.User.Tasks)
            {
                subMenu.Add($"{task.Name} {task.TimeOnTask}", new Action(() =>
                {
                    AppModel.User.SelectTask(task);
                    subMenu.CloseMenu();
                }));
            }
            return subMenu.Add("Back", ConsoleMenu.Close)
                .Configure(config =>
            {
                config.Selector = "--> ";
                config.Title = $"Time";
            });
        }
    }
}
