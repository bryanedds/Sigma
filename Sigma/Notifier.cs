using System.ComponentModel;
using System.Runtime.Serialization;

namespace Sigma
{
    /// <summary>
    /// A mix-in for notifying observers of property changes.
    /// </summary>
    [DataContract]
    public class Notifier : INotifyPropertyChanged
    {
        /// <summary>
        /// Raised when a property is changed.
        /// </summary>
        [field : IgnoreDataMember]
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Hook for handling property change internally in a type.
        /// </summary>
        protected virtual void OnPropertyChanged(string propertyName) { }

        /// <summary>
        /// Notifiy listeners that a property has been changed
        /// </summary>
        protected void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
