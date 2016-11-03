using System.Collections.Generic;
using System.Collections.Specialized;

namespace System.Linq
{
    /// <summary>
    /// TODO: document these functions!
    /// </summary>
    public static class LinqExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> func)
        {
            foreach (var item in enumerable) func(item);
        }

        public static U Then<T, U>(this T value, Func<T, U> func)
        {
            return func(value);
        }

        public static T Require<T>(this T value, Func<T, bool> func)
        {
            if (!func(value)) throw new InvalidOperationException("Value '" + value.ToString() + "' failed requirement.");
            return value;
        }

        public static U TryThen<T, U>(this T value, Func<T, U> func, U default_)
        {
            return value != null ? func(value) : default_;
        }

        public static U TryThen<T, U>(this T value, Func<T, U> func)
        {
            return value.TryThen(func, default(U));
        }

        public static IEnumerable<T> ToIEnumerable<T>(this IEnumerator<T> enumerator)
        {
            while (enumerator.MoveNext()) yield return enumerator.Current;
        }

        public static bool Empty<T>(this IEnumerable<T> enumerable)
        {
            return !enumerable.Any();
        }

        public static bool Empty<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate)
        {
            return !enumerable.Any(predicate);
        }

        public static T FirstOrDefault<T>(this IEnumerable<T> enumerable, T @default)
        {
            if (enumerable.Any()) return enumerable.First();
            return @default;
        }

        public static IEnumerable<T> Append<T>(this IEnumerable<T> enumerable, T value)
        {
            foreach (var cur in enumerable) yield return cur;
            yield return value;
        }

        public static IEnumerable<T> Prepend<T>(this IEnumerable<T> enumerable, T value)
        {
            yield return value;
            foreach (var cur in enumerable) yield return cur;
        }

        public static U Split<T, U>(this IEnumerable<T> enumerable, int span, Func<IEnumerable<T>, IEnumerable<T>, U> continuation)
        {
            // cons up fron values into a list
            var front = new List<T>();
            var enumerator = enumerable.GetEnumerator();
            var i = 0;
            while (enumerator.MoveNext() && i < span)
            {
                var val = enumerator.Current;
                front.Add(val);
                ++i;
            }

            // convert remaining values to enumerable
            var back = enumerator.ToIEnumerable();

            // call continuation on front and back.
            return continuation(front, back);
        }

        public static Tuple<IEnumerable<T>, IEnumerable<T>> Split<T>(this IEnumerable<T> enumerable, int span)
        {
            return Split(enumerable, span, (front, back) => Tuple.Create(front, back));
        }

        public static IEnumerable<IEnumerable<T>> Windowed<T>(this IEnumerable<T> enumerable, int span)
        {
            // ensure span is valid
            if (span < 1) throw new ArgumentException("Must be greater than 0.", nameof(span));

            // call recursive function
            return Windowed(enumerable, span, new List<IEnumerable<T>>());
        }

        public static IEnumerable<T> DistinctBy<T, U, V>(this IEnumerable<T> enumerable, Func<T, U> by, Func<T, V> comparand)
        {
            return enumerable.GroupBy(by).Select(group => group.OrderBy(comparand).First());
        }

        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> enumerable)
        {
            return new HashSet<T>(enumerable);
        }

        public static Dictionary<T, U> ToDictionary<T, U>(this IEnumerable<Tuple<T, U>> enumerable)
        {
            return enumerable.ToDictionary(tuple => tuple.Item1, tuple => tuple.Item2);
        }

        public static Dictionary<T, U> ToDictionarySafe<T, U>(this IEnumerable<Tuple<T, U>> enumerable)
        {
            var dictionary = new Dictionary<T, U>();
            foreach (var entry in enumerable) dictionary[entry.Item1] = entry.Item2;
            return dictionary;
        }

        public static ILookup<T, U> ToLookup<T, U>(this IEnumerable<Tuple<T, U>> enumerable)
        {
            return enumerable.ToLookup(tuple => tuple.Item1, tuple => tuple.Item2);
        }

        public static HashSet<T> Union<T>(this HashSet<T> set, IEnumerable<T> enumerable)
        {
            var union = new HashSet<T>(set);
            union.UnionWith(enumerable);
            return union;
        }

        public static HashSet<T> Intersect<T>(this HashSet<T> set, IEnumerable<T> enumerable)
        {
            var intersection = new HashSet<T>(set);
            intersection.IntersectWith(enumerable);
            return intersection;
        }

        public static HashSet<T> Except<T>(this HashSet<T> set, IEnumerable<T> enumerable)
        {
            var difference = new HashSet<T>(set);
            difference.ExceptWith(enumerable);
            return difference;
        }

        public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> enumerable)
        {
            return enumerable.SelectMany(a => a);
        }

        public static List<T> GetAddedItems<T>(this NotifyCollectionChangedEventArgs e)
        {
            var oldItems = e.OldItems?.Cast<T>() ?? new List<T>();
            var newItems = e.NewItems?.Cast<T>() ?? new List<T>();
            return newItems.Except(oldItems).ToList();
        }

        public static List<T> GetRemovedItems<T>(this NotifyCollectionChangedEventArgs e)
        {
            var oldItems = e.OldItems?.Cast<T>() ?? new List<T>();
            var newItems = e.NewItems?.Cast<T>() ?? new List<T>();
            return oldItems.Except(newItems).ToList();
        }

        public static Tuple<List<T>, List<T>> GetAddedAndRemovedItems<T>(this NotifyCollectionChangedEventArgs e)
        {
            var oldItems = e.OldItems?.Cast<T>() ?? new List<T>();
            var newItems = e.NewItems?.Cast<T>() ?? new List<T>();
            var addedItems = newItems.Except(oldItems).ToList();
            var removedItems = oldItems.Except(newItems).ToList();
            return Tuple.Create(addedItems, removedItems);
        }

        private static IEnumerable<IEnumerable<T>> Windowed<T>(this IEnumerable<T> enumerable, int span, List<IEnumerable<T>> acc)
        {
            return enumerable.Split(span, (front, back) =>
            {
                if (front.Empty()) return acc;
                acc.Add(front);
                return Windowed(back, span, acc);
            });
        }
    }

    public class EqualityComparer<T> : IEqualityComparer<T>
    {
        public EqualityComparer() : this((elem, elem2) => (elem == null && elem2 == null) || (elem != null && elem.Equals(elem2)), t => 0) { }
        public EqualityComparer(Func<T, T, bool> comparer) : this(comparer, t => 0) { }
        public EqualityComparer(Func<T, T, bool> comparer, Func<T, int> hash) { this.comparer = comparer; this.hash = hash; }
        public bool Equals(T x, T y) { return comparer(x, y); }
        public int GetHashCode(T obj) { return hash(obj); }
        private readonly Func<T, T, bool> comparer;
        private readonly Func<T, int> hash;
    }
}
