using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeManager.Model
{
    public class TimeData
    {
        public TimeSpan
        ValueOnStart { get; set; } = TimeSpan.Zero;
        public TimeSpan Previous { get; set; } = TimeSpan.Zero;
        public TimeSpan Current { get; set; } = TimeSpan.Zero;

        public TimeSpan Value => Previous + Current;

        public TimeData()
        {

        }
        public TimeData(TimeSpan previous, TimeSpan current)
        {
            Previous = previous;
            Current = current;
        }
        public void SetTimerValue(TimeSpan previous)
        {

            ValueOnStart = previous;
        }

        public void Reset()
        {
            Previous += Current;

            ValueOnStart = TimeSpan.Zero;
            Current = TimeSpan.Zero;
        }

        public void SetCurrent(TimeSpan newTimerValue)
        {
            Current = newTimerValue -
            ValueOnStart;
        }

        public override string ToString()
        {
            return Watch.FormatTime(Value);
        }
    }
}
