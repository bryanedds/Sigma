using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Sigma
{
    /// <summary>
    /// Event args for the EntryChangedEventHandler.
    /// </summary>
    public class EntryChangedEventArgs<K, V> : EventArgs
    {
        public EntryChangedEventArgs(K key, V value, V oldValue)
        {
            Key = key;
            Value = value;
            OldValue = oldValue;
        }

        public readonly K Key;
        public readonly V Value;
        public readonly V OldValue;
    }

    /// <summary>
    /// Event args for the EntryAddedEventHandler.
    /// </summary>
    public class EntryAddedEventArgs<K, V> : EventArgs
    {
        public EntryAddedEventArgs(K key, V value)
        {
            Key = key;
            Value = value;
        }

        public readonly K Key;
        public readonly V Value;
    }

    /// <summary>
    /// Event args for the EntryRemovedEventHandler.
    /// </summary>
    public class EntryRemovedEventArgs<K, V> : EventArgs
    {
        public EntryRemovedEventArgs(K key, V value)
        {
            Key = key;
            Value = value;
        }

        public readonly K Key;
        public readonly V Value;
    }

    /// <summary>
    /// Handler for changes in entries.
    /// </summary>
    public delegate void EntryChangedEventHandler<K, V>(ObservableDictionary<K, V> sender, EntryChangedEventArgs<K, V> e);

    /// <summary>
    /// Handler for added entries.
    /// </summary>
    public delegate void EntryAddedEventHandler<K, V>(ObservableDictionary<K, V> sender, EntryAddedEventArgs<K, V> e);

    /// <summary>
    /// Handler for removed entries.
    /// </summary>
    public delegate void EntryRemovedEventHandler<K, V>(ObservableDictionary<K, V> sender, EntryRemovedEventArgs<K, V> e);

    /// <summary>
    /// Represents an observable collection of key/value pairs.
    /// </summary>
    /// <typeparam name="K">The type of keys in the dictionary.</typeparam>
    /// <typeparam name="V">The type of values in the dictionary.</typeparam>
    [DataContract]
    public class ObservableDictionary<K, V> : Notifier, IDictionary<K, V>
    {
        public ObservableDictionary()
        {
            // explicit use of data contract serialization requires we initialize our object explicitly...
            Initialize(new StreamingContext());
        }

        /// <summary>
        /// Gets or sets the element with the specified key.
        /// </summary>
        [IgnoreDataMember]
        public V this[K key]
        {
            get { return dictionary[key]; }
            set
            {
                V oldValue;
                var removed = TryGetValue(key, out oldValue);
                if (removed) EntryRemoved?.Invoke(this, new EntryRemovedEventArgs<K, V>(key, oldValue));
                dictionary[key] = value;
                EntryAdded?.Invoke(this, new EntryAddedEventArgs<K, V>(key, value));
                if (removed) EntryChanged?.Invoke(this, new EntryChangedEventArgs<K, V>(key, value, oldValue));
            }
        }

        /// <summary>
        /// Gets the number of elements contained in the dictionary.
        /// </summary>
        public int Count { get { return dictionary.Count; } }

        /// <summary>
        /// Gets a value indicating whether the dictionary is read-only.
        /// </summary>
        public bool IsReadOnly { get { return ((IDictionary<K, V>)dictionary).IsReadOnly; } }

        /// <summary>
        /// Gets an dictionary containing the keys of the dictionary.
        /// </summary>
        public ICollection<K> Keys { get { return dictionary.Keys; } }

        /// <summary>
        /// Gets an dictionary containing the values in the dictionary.
        /// </summary>
        public ICollection<V> Values { get { return dictionary.Values; } }

        /// <summary>
        ///  The contents of the ObservableDictionary
        /// </summary>
        public IReadOnlyDictionary<K, V> Entries { get { return dictionary; } }

        /// <summary>
        /// Raied when an entry is changed.
        /// </summary>
        [field : IgnoreDataMember]
        public event EntryChangedEventHandler<K, V> EntryChanged;

        /// <summary>
        /// Raied when an entry is added.
        /// </summary>
        [field : IgnoreDataMember]
        public event EntryAddedEventHandler<K, V> EntryAdded;

        /// <summary>
        /// Raied when an entry is removed.
        /// </summary>
        [field : IgnoreDataMember]
        public event EntryRemovedEventHandler<K, V> EntryRemoved;

        /// <summary>
        /// Adds an element with the provided key and value to the dictionary.
        /// </summary>
        public void Add(KeyValuePair<K, V> entry)
        {
            ((IDictionary<K, V>)dictionary).Add(entry);
            EntryAdded?.Invoke(this, new EntryAddedEventArgs<K, V>(entry.Key, entry.Value));
        }

        /// <summary>
        /// Adds an entry to the dictionary.
        /// </summary>
        public void Add(K key, V value)
        {
            dictionary.Add(key, value);
            EntryAdded?.Invoke(this, new EntryAddedEventArgs<K, V>(key, value));
        }

        /// <summary>
        /// Removes all entries from the dictionary.
        /// </summary>
        public void Clear()
        {
            var oldKvps = dictionary.ToList();
            dictionary.Clear();
            foreach (var entry in oldKvps) EntryRemoved?.Invoke(this, new EntryRemovedEventArgs<K, V>(entry.Key, entry.Value));
        }

        /// <summary>
        /// Determines whether the dictionary contains a specific value.
        /// </summary>
        public bool Contains(KeyValuePair<K, V> entry)
        {
            return ((IDictionary<K, V>)dictionary).Contains(entry);
        }

        /// <summary>
        /// Determines whether the dictionary contains an element with the specified key.
        /// </summary>
        public bool ContainsKey(K key)
        {
            return dictionary.ContainsKey(key);
        }

        /// <summary>
        /// Copies the elements of the dictionary to an array, starting at a particular array index.
        /// </summary>
        public void CopyTo(KeyValuePair<K, V>[] array, int arrayIndex)
        {
            ((IDictionary<K, V>)dictionary).CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the dictionary.
        /// </summary>
        public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
        {
            return dictionary.GetEnumerator();
        }

        /// <summary>
        /// Removes the element with the specified key from the dictionary.
        /// </summary>
        public bool Remove(KeyValuePair<K, V> entry)
        {
            var removed = ((IDictionary<K, V>)dictionary).Remove(entry);
            if (removed) EntryRemoved?.Invoke(this, new EntryRemovedEventArgs<K, V>(entry.Key, entry.Value));
            return removed;
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the dictionary.
        /// </summary>
        public bool Remove(K key)
        {
            V oldValue;
            TryGetValue(key, out oldValue);
            var removed = ((IDictionary<K, V>)dictionary).Remove(key);
            if (removed) EntryRemoved?.Invoke(this, new EntryRemovedEventArgs<K, V>(key, oldValue));
            return removed;
        }

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        public bool TryGetValue(K key, out V value)
        {
            return dictionary.TryGetValue(key, out value);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the dictionary.
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return dictionary.GetEnumerator();
        }

        public override bool Equals(object obj)
        {
            var dictOpt = obj as ObservableDictionary<K, V>;
            if (dictOpt != null) return this.SequenceEqual(dictOpt);
            return false;
        }

        public override int GetHashCode()
        {
            return dictionary.GetHashCode();
        }

        public override string ToString()
        {
            return dictionary.ToString();
        }

        [OnDeserializing]
        private void Initialize(StreamingContext contextOpt)
        {
            DataContract.Initialize(this, nameof(dictionary), new Dictionary<K, V>());
            EntryAdded += (_, _2) => NotifyPropertyChanged(nameof(Entries));
            EntryRemoved += (_, _2) => NotifyPropertyChanged(nameof(Entries));
        }

        [DataMember]
        private Dictionary<K, V> Dictionary
        {
            get { return dictionary; }
            set
            {
                dictionary.Clear();
                foreach(var entry in value) dictionary.Add(entry.Key, entry.Value);
            }
        }

        #pragma warning disable 0649 // allow reflection-only initialization of these fields
        private readonly Dictionary<K, V> dictionary;
        #pragma warning restore 0649
    }
}
