using System;

namespace Sigma
{
    /// <summary>
    /// Specifies a range of time, that is, a time with a beginning and end.
    /// </summary>
    public class TimeRange : Record<DateTime, DateTime>
    {
        public TimeRange(DateTime startTime, DateTime endTime) : base(startTime, endTime) { }
        public DateTime StartTime { get { return Item1; } }
        public DateTime EndTime { get { return Item2; } }

        /// <summary>
        /// Determine that the given dateTime resides between StartTime and StartTime + TimeSpan.
        /// </summary>
        public bool Contains(DateTime dateTime)
        {
            return
                dateTime >= StartTime &&
                dateTime < EndTime;
        }
    }
}
