using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Sigma
{
    /// <summary>
    /// Event args for the ElementAddedEventHandler.
    /// </summary>
    public class ElementAddedEventArgs<T> : EventArgs
    {
        public ElementAddedEventArgs(T element)
        {
            Element = element;
        }
        
        public readonly T Element;
    }

    /// <summary>
    /// Event args for the ElementRemovedEventHandler.
    /// </summary>
    public class ElementRemovedEventArgs<T> : EventArgs
    {
        public ElementRemovedEventArgs(T element)
        {
            Element = element;
        }
        
        public readonly T Element;
    }

    /// <summary>
    /// Handler for added elements.
    /// </summary>
    public delegate void ElementAddedEventHandler<T>(ObservableHashSet<T> sender, ElementAddedEventArgs<T> e);

    /// <summary>
    /// Handler for removed elements.
    /// </summary>
    public delegate void ElementRemovedEventHandler<T>(ObservableHashSet<T> sender, ElementRemovedEventArgs<T> e);

    /// <summary>
    /// Represents an observable set of elements.
    /// </summary>
    /// <typeparam name="T">The type of elements in the hash set.</typeparam>
    [DataContract]
    public class ObservableHashSet<T> : Notifier, ICollection<T>
    {
        public ObservableHashSet()
        {
            // explicit use of data contract serialization requires we initialize our object explicitly...
            Initialize(new StreamingContext());
        }

        /// <summary>
        /// Gets the number of elements contained in the hash set.
        /// </summary>
        public int Count { get { return hashSet.Count; } }

        /// <summary>
        /// Gets a value indicating whether the hash set is read-only.
        /// </summary>
        public bool IsReadOnly { get { return ((ICollection<T>)hashSet).IsReadOnly; } }

        /// <summary>
        ///  The contents of the ObservableHashSet
        /// </summary>
        public IReadOnlyCollection<T> Elements { get { return hashSet.ToList(); } }

        /// <summary>
        /// Raied when an element is added.
        /// </summary>
        public event ElementAddedEventHandler<T> ElementAdded;

        /// <summary>
        /// Raied when an element is removed.
        /// </summary>
        public event ElementRemovedEventHandler<T> ElementRemoved;

        /// <summary>
        /// Adds an element to the hash set.
        /// </summary>
        public void Add(T element)
        {
            var contains = hashSet.Contains(element);
            ((ICollection<T>)hashSet).Add(element);
            if (!contains) ElementAdded?.Invoke(this, new ElementAddedEventArgs<T>(element));
        }

        /// <summary>
        /// Removes all elements from the hash set.
        /// </summary>
        public void Clear()
        {
            var oldKvps = hashSet.ToList();
            hashSet.Clear();
            foreach(var element in oldKvps) ElementRemoved?.Invoke(this, new ElementRemovedEventArgs<T>(element));
        }

        /// <summary>
        /// Determines whether the hash set contains an element.
        /// </summary>
        public bool Contains(T element)
        {
            return hashSet.Contains(element);
        }

        /// <summary>
        /// Copies the elements of the hash set to an array, starting at a particular array index.
        /// </summary>
        public void CopyTo(T[] array, int arrayIndex)
        {
            ((ICollection<T>)hashSet).CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the hash set.
        /// </summary>
        public IEnumerator<T> GetEnumerator()
        {
            return hashSet.GetEnumerator();
        }

        /// <summary>
        /// Removes the element from the hash set.
        /// </summary>
        public bool Remove(T element)
        {
            var removed = ((ICollection<T>)hashSet).Remove(element);
            if (removed) ElementRemoved?.Invoke(this, new ElementRemovedEventArgs<T>(element));
            return removed;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the hash set.
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return hashSet.GetEnumerator();
        }

        public override bool Equals(object obj)
        {
            var optHashSet = obj as ObservableHashSet<T>;
            if (optHashSet != null) return this.SequenceEqual(optHashSet);
            return false;
        }

        public override int GetHashCode()
        {
            return hashSet.GetHashCode();
        }

        public override string ToString()
        {
            return hashSet.ToString();
        }

        [OnDeserializing]
        private void Initialize(StreamingContext optContext)
        {
            DataContract.Initialize(this, nameof(hashSet), new HashSet<T>());
            ElementAdded += (_, _2) => NotifyPropertyChanged(nameof(Elements));
            ElementRemoved += (_, _2) => NotifyPropertyChanged(nameof(Elements));
        }

        [DataMember]
        private HashSet<T> HashSet
        {
            get { return hashSet; }
            set
            {
                hashSet.Clear();
                foreach(var element in value) hashSet.Add(element);
            }
        }

        #pragma warning disable 0649 // allow reflection-only initialization of these fields
        private readonly HashSet<T> hashSet;
        #pragma warning restore 0649
    }
}
