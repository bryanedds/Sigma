using System;
using System.Runtime.Serialization;
using System.Windows.Threading;

namespace Sigma
{
    /// <summary>
    /// Represents a date time object that notifies when its time value is internally updated.
    /// </summary>
    [DataContract]
    public class ObservableDateTime : Notifier
    {
        public ObservableDateTime()
        {
            Initialize(new StreamingContext());
        }

        [DataMember]
        public double Granularity
        {
            get { return timer.Interval.TotalSeconds; }
            set { timer.Interval = TimeSpan.FromSeconds(value); }
        }

        public DateTime Now
        {
            get { return now; }
        }

        public DateTime UtcNow
        {
            get { return now; }
        }

        [OnDeserializing]
        private void Initialize(StreamingContext context)
        {
            DataContract.Initialize(this, nameof(timer), new DispatcherTimer());
            timer.Interval = TimeSpan.FromSeconds(250.0f);
            timer.Tick += (_, _2) =>
            {
                now = DateTime.Now;
                utcNow = DateTime.UtcNow;
                NotifyPropertyChanged(nameof(Now));
                NotifyPropertyChanged(nameof(UtcNow));
            };
            timer.Start();
            now = DateTime.Now;
            utcNow = DateTime.UtcNow;
        }

        #pragma warning disable 0649 // allow reflection-only initialization of these fields
        private readonly DispatcherTimer timer;
        #pragma warning restore 0649
        private DateTime now;
        private DateTime utcNow;
    }
}
