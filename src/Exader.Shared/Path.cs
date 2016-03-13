using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;

namespace Exader
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class Path<T> : IPathList<T>
    {
        public static implicit operator T[](Path<T> path)
        {
            return path.items;
        }

        public static implicit operator Path<T>(T[] array)
        {
            return new Path<T>(array);
        }

        private T[] items;

        public Path(IEnumerable<T> collection)
        {
            items = collection.ToArray();
        }

        public Path(params T[] array)
        {
            items = array ?? Empty<T>.Array;
        }

        public Path(T leaf, Func<T, T> getParent, Func<T, bool> accept = null)
        {
            var list = new List<T>();
            while (!Equals(default(T), leaf) && ((null == accept) || accept(leaf)))
            {
                list.Add(leaf);
                leaf = getParent(leaf);
            }

            list.Reverse();
            items = list.ToArray();
        }

        internal Path(T[] sourceArray, int start, int length)
        {
            if (null == sourceArray) throw new ArgumentNullException(nameof(sourceArray));

            items = new T[length];
            Array.Copy(sourceArray, start, items, 0, length);
        }

        public override string ToString()
        {
            return ToString("/");
        }

        public void AddLeaf(T[] array)
        {
            items = items.AddRange(array);
        }

        public void AddLeaf(IEnumerable<T> collection)
        {
            items = items.AddRange(collection);
        }

        public void AddRoot(T[] array)
        {
            items = items.AddRange(array);
        }

        public void AddRoot(IEnumerable<T> collection)
        {
            items = items.AddRange(collection);
        }

        public string ToString(string separator, bool relative = false)
        {
            if (Count == 0) return string.Empty;

            var buffer = new StringBuilder();
            foreach (T item in items)
            {
                if (!relative) buffer.Append(separator);
                relative = false;

                buffer.Append(item);
            }

            return buffer.ToString();
        }

        private void CheckIndex(int index)
        {
            if (index < 0 || Count <= index)
            {
                throw new IndexOutOfRangeException("index");
            }
        }

        #region IPathList<T> Members

        public T this[int offset]
        {
            get
            {
                if (offset < 0)
                {
                    offset = Count + offset;
                }

                CheckIndex(offset);

                return items[offset];
            }
        }

        IPath<T> IPath<T>.this[int offset, int count]
        {
            get
            {
                if (offset < 0)
                {
                    offset = Count + offset;
                }

                if (count < 0)
                {
                    offset = offset + count;
                    count = Math.Abs(count);
                }

                CheckIndex(offset);

                var array = new T[count];
                Array.Copy(items, offset, array, 0, count);
                return new Path<T>(array);
            }
        }

        public int Count
        {
            get { return (null == items) ? 0 : items.Length; }
        }

        public T Leaf
        {
            get { return 0 == Count ? default(T) : this[items.Length - 1]; }
        }

        public T Root
        {
            get { return 0 == Count ? default(T) : items[0]; }
        }

        public void AddLeaf(T item)
        {
            items = items.Add(item);
        }

        public void AddRoot(T item)
        {
            items = items.AddFirst(item);
        }

        public void Clear()
        {
            items = Empty<T>.Array;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)items).GetEnumerator();
        }

        public void RemoveLeaf(int count = 1)
        {
            CheckIndex(count);

            items = items.Subarray(0, items.Length - count);
        }

        public void RemoveRoot(int count = 1)
        {
            items = items.Subarray(count);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
    
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
#if NET35
    public interface IPath<T> : IEnumerable<T>
#else
    public interface IPath<out T> : IEnumerable<T>
#endif
    {
        IPath<T> this[int offset, int count] { get; }

        T this[int offset] { get; }

        int Count { get; }

        T Leaf { get; }

        T Root { get; }
    }

    public interface IPathList<T> : IPath<T>
    {
        void AddLeaf(T item);

        void AddRoot(T item);

        void Clear();

        void RemoveLeaf(int count);

        void RemoveRoot(int count);
    }
}