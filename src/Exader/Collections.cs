using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using JetBrains.Annotations;

namespace Exader
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public static class EnumerableExtensions
    {
        public static bool IsNullOrEmpty(this IEnumerable self)
        {
            return self == null || !self.GetEnumerator().MoveNext();
        }

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> self, Func<T, bool> predicate)
        {
            return self == null || !self.Any(predicate);
        }

        public static bool IsSingleton<T>(this IEnumerable<T> self, Func<T, bool> predicate)
        {
            if (self != null)
            {
                var t = self.SingletonOrDefault();
                return !Equals(t, default(T)) && predicate(t);
            }

            return false;
        }

        public static T SingletonOrDefault<T>(this IEnumerable<T> self)
        {
            if (self != null)
            {
                var list = self as IList<T>;
                if (list != null)
                {
                    if (list.Count == 1)
                    {
                        return list[0];
                    }
                }
                else
                {
                    using (IEnumerator<T> enumerator = self.GetEnumerator())
                    {
                        if (enumerator.MoveNext())
                        {
                            T result = enumerator.Current;
                            if (!enumerator.MoveNext()) return result;
                        }
                    }
                }
            }

            return default(T);
        }

        // в большинстве случаев использования все равно возвращаемое значение не используется, а тут оно еще и не ведет себя ожидаемо, из-за multiple enumeration
        // поэтому переделал на void
        public static void ForEach<T>(this IEnumerable<T> enumerableT, Action<T> action)
        {
            if (action != null && enumerableT != null)
            {
                foreach (T t in enumerableT)
                {
                    action(t);
                }
            }
        }

        public static IEnumerable<TResult> LeftJoin<TOuter, TInner, TKey, TResult>(
            this IEnumerable<TOuter> outer,
            IEnumerable<TInner> inner,
            Func<TOuter, TKey> outerKeySelector,
            Func<TInner, TKey> innerKeySelector,
            Func<TOuter, TInner, TResult> resultSelector)
        {
            var qq = from o in outer
                     join i in inner on outerKeySelector(o) equals innerKeySelector(i) into j
                     from jj in j.DefaultIfEmpty()
                     select resultSelector(o, jj);
            return qq;
        }

        public static IEnumerable<TResult> RightJoin<TOuter, TInner, TKey, TResult>(
            this IEnumerable<TOuter> outer,
            IEnumerable<TInner> inner,
            Func<TOuter, TKey> outerKeySelector,
            Func<TInner, TKey> innerKeySelector,
            Func<TOuter, TInner, TResult> resultSelector)
        {
            var qq = from i in inner
                     join o in outer on innerKeySelector(i) equals outerKeySelector(o) into j
                     from jj in j.DefaultIfEmpty()
                     select resultSelector(jj, i);
            return qq;
        }

        public static IEnumerable<TResult> FullJoin<TOuter, TInner, TKey, TResult>(
            this IEnumerable<TOuter> outer,
            IEnumerable<TInner> inner,
            Func<TOuter, TKey> outerKeySelector,
            Func<TInner, TKey> innerKeySelector,
            Func<TOuter, TInner, TResult> resultSelector)
        {
            var outerList = outer.ToList();
            var innerList = inner.ToList();

            var leftJoin = outerList.LeftJoin(innerList, outerKeySelector, innerKeySelector, resultSelector);
            var rightJoin = outerList.RightJoin(innerList, outerKeySelector, innerKeySelector, resultSelector);

            return leftJoin.Union(rightJoin);
        }

        public static IEnumerable<IEnumerable<T>> AsBatches<T>(this IEnumerable<T> self, int batchSize)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            // ReSharper disable once PossibleMultipleEnumeration
            var batchesCount = (int)Math.Ceiling(self.Count() / (double)batchSize);

            var list = self as IList<T>;
            if (list != null)
            {
                for (int batchIndex = 0; batchIndex < batchesCount; batchIndex++)
                {
                    yield return new ListWindowEnumerator<T>(list, batchIndex * batchSize, batchSize).AsEnumerable();
                }
            }
            else
            {
                var array = self as IList;
                if (array != null)
                {
                    for (int batchIndex = 0; batchIndex < batchesCount; batchIndex++)
                    {
                        yield return new ArrayWindowEnumerator<T>(array, batchIndex * batchSize, batchSize).AsEnumerable();
                    }
                }
                else
                {
                    for (int batchIndex = 0; batchIndex < batchesCount; batchIndex++)
                    {
                        // ReSharper disable once PossibleMultipleEnumeration
                        yield return Window(self, batchIndex * batchSize, batchSize);
                    }
                }
            }
        }

        public static IEnumerable<T> AsEnumerable<T>(this IEnumerator<T> self)
        {
            return self == null ? Enumerable.Empty<T>() : new Enumerable<T>(self);
        }

        private static IEnumerable<T> Window<T>(IEnumerable<T> source, int startIndex, int count)
        {
            using (IEnumerator<T> e = source.GetEnumerator())
            {
                while (0 < startIndex && e.MoveNext()) startIndex--;

                while (0 < count && e.MoveNext())
                {
                    count--;
                    yield return e.Current;
                }
            }
        }

        public struct Enumerable<T> : IEnumerable<T>
        {
            private readonly IEnumerator<T> _enumerator;

            public Enumerable(IEnumerator<T> enumerator)
            {
                _enumerator = enumerator;
            }

            public IEnumerator<T> GetEnumerator() => _enumerator;
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        public struct ListWindowEnumerator<T> : IEnumerator<T>
        {
            private readonly IList<T> _source;
            private readonly int _startIndex;
            private readonly int _endIndex;
            private int _current;

            public ListWindowEnumerator(IList<T> source, int startIndex, int count)
            {
                _source = source;
                _startIndex = startIndex - 1;
                _current = _startIndex;
                _endIndex = Math.Min(startIndex + count, source.Count);
            }

            public bool MoveNext()
            {
                _current++;
                return _current < _endIndex;
            }

            public void Reset()
            {
                _current = _startIndex;
            }

            object IEnumerator.Current => Current;

            public T Current
            {
                get
                {
                    if (_current == _startIndex)
                    {
                        return default(T);
                    }

                    return _source[_current];
                }
            }

            public void Dispose()
            {
                Reset();
            }
        }

        public struct ArrayWindowEnumerator<T> : IEnumerator<T>
        {
            private readonly IList _source;
            private readonly int _startIndex;
            private readonly int _endIndex;
            private int _current;

            public ArrayWindowEnumerator(IList source, int startIndex, int count)
            {
                _source = source;
                _startIndex = startIndex - 1;
                _current = startIndex;
                _endIndex = Math.Min(startIndex + count, source.Count);
            }

            public bool MoveNext()
            {
                _current++;
                return _current < _endIndex;
            }

            public void Reset()
            {
                _current = _startIndex;
            }

            object IEnumerator.Current => Current;

            public T Current
            {
                get
                {
                    if (_current == _startIndex)
                    {
                        return default(T);
                    }

                    return (T)_source[_current];
                }
            }

            public void Dispose()
            {
                Reset();
            }
        }
    }

    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public static class DictionaryExtensions
    {
        public static TValue GetValueOrDefault<TKey, TValue>(
            this IDictionary<TKey, TValue> self, TKey? key, TValue defaultValue = null)
            where TKey : struct
            where TValue : class
        {
            TValue value;
            return key.HasValue && self.TryGetValue(key.Value, out value)
                ? value : defaultValue;
        }

        public static TValue GetValueOrDefault<TKey, TValue>(
            this IDictionary<TKey, TValue> self, TKey key, TValue defaultValue = null)
            where TValue : class
        {
            TValue value;
            return self.TryGetValue(key, out value) ? value : defaultValue;
        }

        public static TResult GetConvertedValueOrDefault<TKey, TValue, TResult>(
            this IDictionary<TKey, TValue> self, TKey key, TResult defaultValue, IFormatProvider provider = null)
        {
            TValue value;
            if (self.TryGetValue(key, out value))
            {
                if (provider == null)
                {
                    provider = CultureInfo.InvariantCulture;
                }

                return (TResult)Convert.ChangeType(value, typeof(TResult), provider);
            }

            return defaultValue;
        }

        public static TResult GetConvertedValue<TResult>(
            this IDictionary<string, object> self, string key, TResult defaultValue, IFormatProvider provider = null)
        {
            object value;
            if (self.TryGetValue(key, out value))
            {
                if (provider == null)
                {
                    provider = CultureInfo.InvariantCulture;
                }

                return (TResult)Convert.ChangeType(value, typeof(TResult), provider);
            }

            return defaultValue;
        }

        public static TResult GetConvertedValue<TResult>(
            this IDictionary<string, string> self, string key, TResult defaultValue, IFormatProvider provider = null)
        {
            string value;
            if (self.TryGetValue(key, out value))
            {
                if (provider == null)
                {
                    provider = CultureInfo.InvariantCulture;
                }

                return (TResult)Convert.ChangeType(value, typeof(TResult), provider);
            }

            return defaultValue;
        }

        public static IEnumerable<TValue> GetExistingValuesOf<TKey, TValue>(
            this IDictionary<TKey, TValue> self, IEnumerable<TKey> keys)
            where TValue : class
        {
            if (keys == null) yield break;

            foreach (var key in keys)
            {
                TValue value;
                if (self.TryGetValue(key, out value))
                {
                    yield return value;
                }
            }
        }

        public static IEnumerable<TValue> GetExistingValuesOf<TKey, TValue>(
            this IDictionary<TKey, TValue> self, IEnumerable<TKey?> keys)
            where TKey : struct
            where TValue : class
        {
            if (keys == null) yield break;

            foreach (var key in keys)
            {
                TValue value;
                if (key.HasValue && self.TryGetValue(key.Value, out value))
                {
                    yield return value;
                }
            }
        }

        public static IEnumerable<TValue> GetValuesOf<TKey, TValue>(
            this IDictionary<TKey, TValue> self,
            IEnumerable<TKey> keys,
            TValue defaultValue = null)
            where TValue : class
        {
            if (keys == null) yield break;

            foreach (var key in keys)
            {
                TValue value;
                yield return self.TryGetValue(key, out value)
                    ? value : defaultValue;
            }
        }

        public static IEnumerable<TValue> GetValuesOf<TKey, TValue>(
            this IDictionary<TKey, TValue> self, IEnumerable<TKey?> keys, TValue defaultValue = null)
            where TKey : struct
            where TValue : class
        {
            if (keys == null) yield break;

            foreach (var key in keys)
            {
                if (!key.HasValue)
                {
                    yield return defaultValue;
                    continue;
                }

                TValue value;
                yield return self.TryGetValue(key.Value, out value)
                    ? value : defaultValue;
            }
        }

        public static IEnumerable<TValue> GetValuesOf<TKey, TValue>(
            this IDictionary<TKey, TValue> self, IEnumerable<TKey?> keys, Func<TKey, TValue> fallbackSelector, TValue defaultValue = null)
            where TKey : struct
            where TValue : class
        {
            if (keys == null) yield break;

            foreach (var key in keys)
            {
                if (!key.HasValue)
                {
                    yield return defaultValue;
                    continue;
                }

                TValue value;
                yield return self.TryGetValue(key.Value, out value)
                    ? value : fallbackSelector(key.Value);
            }
        }

        public static IEnumerable<TValue> GetValuesOf<TKey, TValue>(
            this IDictionary<TKey, TValue> self, IEnumerable<TKey> keys, Func<TKey, TValue> fallbackSelector)
            where TValue : class
        {
            if (keys == null) yield break;

            foreach (var key in keys)
            {
                TValue value;
                yield return self.TryGetValue(key, out value)
                    ? value : fallbackSelector(key);
            }
        }

        public static TBase GetValueOrKeyOf<TKey, TValue, TBase>(
            this IDictionary<TKey, TValue> self, TKey key, TBase defaultValue = null)
            where TKey : TBase
            where TValue : TBase
            where TBase : class
        {
            TValue value;
            return self.TryGetValue(key, out value)
                ? (TBase)value : key;
        }

        public static IEnumerable<TBase> GetValuesOrKeysOf<TKey, TValue, TBase>(
            this IDictionary<TKey, TValue> self, IEnumerable<TKey> keys)
            where TKey : TBase
            where TValue : TBase
            where TBase : class
        {
            if (keys == null) yield break;

            foreach (var key in keys)
            {
                TValue value;
                yield return self.TryGetValue(key, out value)
                    ? (TBase)value : key;
            }
        }

        public static bool TryGetCastedValue<TKey, TValue>(
            this IDictionary<TKey, object> dictionary,
            TKey key,
            out TValue value)
            where TValue : class
        {
            object o;
            if (!dictionary.TryGetValue(key, out o))
            {
                value = default(TValue);
                return false;
            }

            return (value = o as TValue) != null;
        }
    }
}
