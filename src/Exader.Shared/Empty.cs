using System.Collections;
using System.Collections.Generic;

namespace Exader
{
    /// <summary>
    /// Поставляет свойства с пустым значением коллекций.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class Empty<T>
    {
        private static readonly T[] EmptyArray;

        private static readonly IEnumerator<T> EmptyEnumerator;

        static Empty()
        {
            EmptyArray = new T[0];
            EmptyEnumerator = ((IEnumerable<T>)EmptyArray).GetEnumerator();
        }

        /// <summary>
        /// Возвращает экземпляр пустого массива.
        /// </summary>
        public static T[] Array => EmptyArray;

        public static ICollection<T> Collection => EmptyArray;
    }

    /// <summary>
    /// Поставляет свойства с пустым значением коллекций.
    /// </summary>
    public static class Empty
    {
        private static readonly object[] EmptyArray;

        private static readonly IEnumerator EmptyEnumerator;

        static Empty()
        {
            EmptyArray = new object[0];
            EmptyEnumerator = ((IEnumerable)EmptyArray).GetEnumerator();
        }

        /// <summary>
        /// Возвращает экземпляр пустого массива.
        /// </summary>
        public static object[] Array => EmptyArray;

        public static ICollection Collection => EmptyArray;

        public static IEnumerable Enumerable => EmptyArray;

        public static IEnumerator Enumerator => EmptyEnumerator;

        public static IList List => EmptyArray;
    }
}
