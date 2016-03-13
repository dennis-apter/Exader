using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Exader
{
    /// <summary>
    /// Расширяет поведение массива добавляя методы манипуляции его элементами.
    /// </summary>
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public static class ArrayExtensions
    {
        /// <summary>
        /// Добавляет элемент в конец массива.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="item"></param>
        [Pure]
        public static T[] Add<T>(this T[] self, T item)
        {
            if (null == self)
            {
                return new[] { item };
            }

            self = self.Growth(1);
            self[self.Length - 1] = item;
            return self;
        }
        
        [Pure]
        public static T[] Add<T>(this T[] self, params T[] items)
        {
            if (null == self)
            {
                return items;
            }

            return self.Insert(self.Length, items);
        }
        
        [Pure]
        public static T[] AddFirst<T>(this T[] self, T item)
        {
            if (null == self)
            {
                return new[] { item };
            }

            self = self.Shift(1);
            self[0] = item;
            return self;
        }
        
        [Pure]
        public static T[] AddRange<T>(this T[] self, IEnumerable<T> enumerable)
        {
            if (null == self)
            {
                return enumerable.ToArray();
            }

            return self.InsertRange(self.Length, enumerable);
        }

        /// <summary>
        /// Возвращает значение характеризующее наличие элемента в массиве.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        [Pure]
        public static bool Contains<T>(this T[] self, T item)
        {
            return 0 <= Array.IndexOf(self, item);
        }
        
        [Pure]
        public static bool Contains<T>(this T[] self, T item, bool useBinarySearch, bool sort)
        {
            if (sort)
            {
                Array.Sort(self);
            }

            if (useBinarySearch)
            {
                return 0 <= Array.BinarySearch(self, item);
            }

            return 0 <= Array.IndexOf(self, item);
        }
        
        [Pure]
        public static bool Contains<T>(this T[] self, T item, bool useBinarySearch, bool sort, IComparer comparer)
        {
            if (sort)
            {
                Array.Sort(self);
            }

            if (useBinarySearch)
            {
                return 0 <= Array.BinarySearch(self, 0, self.Length, item, comparer);
            }

            return 0 <= Array.IndexOf(self, item);
        }

        /// <summary>
        /// Обеспечивает заданный размер массива.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="newSize"></param>
        /// <param name="fill">Заполнитель пустых ячеек массива.</param>
        public static T[] EnsureSize<T>(this T[] self, int newSize, T fill)
        {
            if (self.Length != newSize)
            {
                int oldSize = self.Length;

                Array.Resize(ref self, newSize);

                if (oldSize < newSize)
                {
                    for (int i = oldSize; i < newSize; i++)
                    {
                        self[i] = fill;
                    }
                }
            }

            return self;
        }

        /// <summary>
        /// Обеспечивает заданный размер массива.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="newSize"></param>
        public static T[] EnsureSize<T>(this T[] self, int newSize)
        {
            if (self.Length != newSize)
            {
                Array.Resize(ref self, newSize);
            }

            return self;
        }
        
        public static void Expand<T>(this T[] array, int offset, int startIndex)
        {
            if (startIndex < 0)
            {
                array.Expand(offset, array.Length + startIndex - 1, -startIndex + 1);
            }
            else
            {
                array.Expand(offset, startIndex, array.Length - startIndex);
            }
        }

        /// <summary>
        /// Раздвигает элементы в массиве.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="offset">Дистанция смещения.</param>
        /// <param name="startIndex">Начальный индекс сдвига.</param>
        /// <param name="length">Длина сдвига.</param>
        public static void Expand<T>(this T[] array, int offset, int startIndex, int length)
        {
            if (length == 0)
            {
                return;
            }

            if (offset == 0)
            {
                return;
            }

            if (length < 0)
            {
                length = 0 - length;
                startIndex -= length - 1;
                if (startIndex < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(length));
                }
            }
            else
            {
                if ((array.Length - startIndex) < length)
                {
                    throw new ArgumentOutOfRangeException(nameof(length));
                }
            }

            if (length <= Math.Abs(offset))
            {
                throw new ArgumentOutOfRangeException(nameof(offset));
            }

            if (offset < 0)
            {
                int boundary = (startIndex + length) + offset - 1;
                for (int i = startIndex; i < boundary; i++)
                {
                    array[i] = array[i - offset];
                }

                int endIndex = startIndex + length - 1;
                for (int i = boundary; i < endIndex; i++)
                {
                    array[i] = array[endIndex];
                }
            }
            else
            {
                int boundary = startIndex + offset;
                for (int i = startIndex + length - 1; boundary < i; i--)
                {
                    array[i] = array[i - offset];
                }

                for (int i = startIndex + 1; i <= boundary; i++)
                {
                    array[i] = array[startIndex];
                }
            }
        }

        /// <summary>
        /// Сдвигает элементы массива за счёт заданного элемента для удаления.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="collapsible">Элемент для удаления из массива</param>
        /// <returns></returns>
        [Pure]
        public static T[] Collapse<T>(this T[] self, T collapsible = default(T))
        {
            int offset = 0;
            for (int i = 0; i < self.Length; i++)
            {
                if (Equals(self[i], collapsible)) continue;

                self[offset] = self[i];
                offset++;
            }

            return self.Resize(offset);
        }

        public static void Fill<T>(this T[] self, T value)
        {
            self.Fill(value, 0, self.Length);
        }

        public static void Fill<T>(this T[] self, T[] values)
        {
            Fill(self, values, 0, self.Length);
        }

        public static void Fill<T>(this T[] self, IEnumerable<T> values)
        {
            Fill(self, values, 0, self.Length);
        }

        public static void Fill<T>(this T[] self, IEnumerable<T> values, int startIndex, int count)
        {
            if (null == values)
            {
                throw new ArgumentNullException(nameof(values));
            }

            // ReSharper disable once PossibleMultipleEnumeration
            IEnumerator<T> enumerator = values.GetEnumerator();
            if (enumerator.MoveNext())
            {
                int endIndex = startIndex + count;
                for (int i = startIndex; i < endIndex; i++)
                {
                    self[i] = enumerator.Current;

                    if (!enumerator.MoveNext())
                    {
                        // ReSharper disable once PossibleMultipleEnumeration
                        enumerator = values.GetEnumerator();
                        enumerator.MoveNext();
                    }
                }
            }
        }

        public static void Fill<T>(this T[] self, T[] values, int startIndex, int count)
        {
            if (null == values)
            {
                throw new ArgumentNullException(nameof(values));
            }

            int endIndex = startIndex + count;
            int j = 0;
            for (int i = startIndex; i < endIndex; i++)
            {
                self[i] = values[j++];

                if (j == values.Length)
                {
                    j = 0;
                }
            }
        }

        public static void Fill<T>(this T[] self, T value, int startIndex, int count)
        {
            int endIndex = startIndex + count;
            for (int i = startIndex; i < endIndex; i++)
            {
                self[i] = value;
            }
        }

        /// <summary>
        /// Увеличивает размер массива на заданное количество элементов.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="amount"></param>
        [Pure]
        public static T[] Growth<T>(this T[] self, int amount)
        {
            Array.Resize(ref self, self.Length + amount);
            return self;
        }
        
        [Pure]
        public static int IndexOf<T>(this T[] self, T item, int startIndex)
        {
            return Array.IndexOf(self, item, startIndex);
        }
        
        [Pure]
        public static int IndexOf<T>(this T[] self, T item)
        {
            return Array.IndexOf(self, item);
        }

        /// <summary>
        /// Вставляет массив элементов в заданную позицию.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="index"></param>
        /// <param name="items"></param>
        public static T[] Insert<T>(this T[] self, int index, params T[] items)
        {
            if (null == items)
            {
                throw new ArgumentNullException(nameof(items));
            }

            if (self.Length < index)
            {
#if SILVERLIGHT
                throw new ArgumentOutOfRangeException("index", "Позиция вставки превышает длину массива.");
#else
                throw new ArgumentOutOfRangeException(nameof(index), index, "Позиция вставки превышает длину массива.");
#endif
            }

            int length = items.Length;
            if (0 < length)
            {
                int capacity = self.Length + length;
                if (index < (self.Length - 1))
                {
                    self = self.EnsureSize(capacity);
                    Array.Copy(self, index, self, (index + length + 1), (self.Length - index - 1));
                }

                self = self.EnsureSize(capacity);
                if (self == items)
                {
                    // Дублирование элементов массива

                    Array.Copy(self, 0, self, index, index);
                    Array.Copy(self, (index + length), self, (2 * index), (self.Length - index));
                }
                else
                {
                    items.CopyTo(self, index);
                }
            }

            return self;
        }

        /// <summary>
        /// Вставляет элемент в заданную позицию.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="index"></param>
        /// <param name="item"></param>
        public static T[] Insert<T>(this T[] self, int index, T item)
        {
            if (self.Length < index)
            {
#if SILVERLIGHT
                throw new ArgumentOutOfRangeException("index", "Позиция вставки превышает длину массива.");
#else
                throw new ArgumentOutOfRangeException(nameof(index), index, "Позиция вставки превышает длину массива.");
#endif
            }

            self = self.Growth(1);
            if (index < (self.Length - 1))
            {
                Array.Copy(self, index, self, (index + 1), (self.Length - index - 1));
            }

            self[index] = item;
            return self;
        }

        /// <summary>
        /// Вставляет перечисление элементов в заданную позицию.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="index"></param>
        /// <param name="enumerable"></param>
        public static T[] InsertRange<T>(this T[] self, int index, IEnumerable<T> enumerable)
        {
            if (null == enumerable)
            {
                throw new ArgumentNullException(nameof(enumerable));
            }

            if (self.Length < index)
            {
#if SILVERLIGHT
                throw new ArgumentOutOfRangeException("index", "Позиция вставки превышает длину массива.");
#else
                throw new ArgumentOutOfRangeException(nameof(index), index, "Позиция вставки превышает длину массива.");
#endif
            }

            var collection = enumerable as ICollection<T>;
            if (null == collection)
            {
                int offset = 0;
                int capacity = 4;
                var array = new T[capacity];
                foreach (T item in enumerable)
                {
                    if (capacity < offset)
                    {
                        array.Growth(capacity);
                        capacity += capacity;
                    }

                    array[offset++] = item;
                }

                self = self.Insert(index, array.EnsureSize(offset));
            }
            else
            {
                int count = collection.Count;
                if (0 < count)
                {
                    int capacity = self.Length + count;
                    if (index < (self.Length - 1))
                    {
                        self = self.EnsureSize(capacity);
                        Array.Copy(self, (index + 1), self, (index + count), (self.Length - index - 1));
                    }

                    self = self.EnsureSize(capacity);
                    if (self == collection)
                    {
                        // Дублирование элементов массива

                        Array.Copy(self, 0, self, index, index);
                        Array.Copy(self, (index + count), self, (2 * index), (self.Length - index));
                    }
                    else
                    {
                        var array = new T[count];
                        collection.CopyTo(array, 0);
                        array.CopyTo(self, index);
                    }
                }
            }

            return self;
        }
        
        [Pure]
        public static int LastIndexOf<T>(this T[] self, T item, int startIndex)
        {
            return Array.LastIndexOf(self, item, startIndex);
        }
        
        [Pure]
        public static int LastIndexOf<T>(this T[] self, T item)
        {
            return Array.LastIndexOf(self, item);
        }

        /// <summary>
        /// Удаляет элемент массива из первой найденной позиции.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="item"></param>
        public static T[] Remove<T>(this T[] self, T item)
        {
            if (Equals(default(T), item))
            {
                throw new ArgumentNullException(nameof(item));
            }

            int index = Array.IndexOf(self, item);
            if (0 <= index)
            {
                self = self.RemoveAt(index);
            }

            return self;
        }

        /// <summary>
        /// Удаляет элемент массива из последней найденной позиции.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="item"></param>
        [Pure]
        public static T[] RemoveLast<T>(this T[] self, T item)
        {
            if (Equals(default(T), item))
            {
                throw new ArgumentNullException(nameof(item));
            }

            int index = Array.LastIndexOf(self, item);
            if (0 <= index)
            {
                self = self.RemoveAt(index);
            }

            return self;
        }

        /// <summary>
        /// Удаляет элемент из массива в заданной позиции.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="index"></param>
        [Pure]
        public static T[] RemoveAt<T>(this T[] self, int index)
        {
            if (self.Length <= index)
            {
#if SILVERLIGHT
                throw new ArgumentOutOfRangeException("index", "Позиция удаления превышает длину массива.");
#else
                throw new ArgumentOutOfRangeException(nameof(index), index, "Позиция удаления превышает длину массива.");
#endif
            }

            if (index < (self.Length - 1))
            {
                Array.Copy(self, (index + 1), self, index, (self.Length - index - 1));
            }

            return self.Shrink(1);
        }

        /// <summary>
        /// Изменяет размер массива до заданного.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="newSize"></param>
        [JetBrains.Annotations.Pure]
        public static T[] Resize<T>(this T[] array, int newSize)
        {
            Array.Resize(ref array, newSize);
            return array;
        }

        /// <summary>
        /// Сдвигаяет элементы массива на заданное смещение изменяя его размер.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static T[] Shift<T>(this T[] array, int offset)
        {
            if (offset != 0)
            {
                if (offset < 0)
                {
                    if (array.Length < -offset)
                    {
                        throw new ArgumentOutOfRangeException(nameof(offset));
                    }

                    int length = array.Length + offset;
                    Array.Copy(array, -offset, array, 0, length);
                    Array.Resize(ref array, length);
                }
                else
                {
                    int length = array.Length;
                    Array.Resize(ref array, array.Length + offset);
                    Array.Copy(array, 0, array, offset, length);
                }
            }

            return array;
        }

        /// <summary>
        /// Сдвигаяет элементы массива на заданное смещение неизменяя его размер.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        [Pure]
        public static T[] Move<T>(this T[] array, int offset)
        {
            if (offset != 0)
            {
                if (offset < 0)
                {
                    if (array.Length < -offset)
                    {
                        throw new ArgumentOutOfRangeException(nameof(offset));
                    }

                    int length = array.Length + offset;
                    Array.Copy(array, -offset, array, 0, length);
                }
                else
                {
                    int length = array.Length;
                    Array.Copy(array, 0, array, offset, length - offset);
                }
            }

            return array;
        }

        /// <summary>
        /// Сокращает размер массива на заданное количество элементов.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="amount"></param>
        public static T[] Shrink<T>(this T[] self, int amount)
        {
            int newSize = self.Length - amount;
            newSize = newSize < 0 ? 0 : newSize;
            Array.Resize(ref self, newSize);
            return self;
        }
        
        [Pure]
        public static T[] Subarray<T>(this T[] array, int startIndex)
        {
            int length = array.Length;
            var dest = new T[length - startIndex];
            Array.Copy(array, startIndex, dest, 0, dest.Length);
            return dest;
        }
        
        [Pure]
        public static T[] Subarray<T>(this T[] array, int startIndex, int length)
        {
            var dest = new T[length];
            Array.Copy(array, startIndex, dest, 0, length);
            return dest;
        }
        
        [Pure]
        public static T[] Subarray<T>(this T[] array, int startIndex, int length, int destinationLength)
        {
            var dest = new T[destinationLength];
            Array.Copy(array, startIndex, dest, 0, length);
            return dest;
        }
    }
}
