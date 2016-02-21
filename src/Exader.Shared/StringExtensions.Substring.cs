using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using JetBrains.Annotations;

namespace Exader
{
    public static partial class StringExtensions
    {
        /// <summary>
        ///     Возвращает строку после заданной позиции.
        /// </summary>
        /// <param name="self"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        [NotNull]
        public static string SubstringAfter([CanBeNull] this string self, int offset)
        {
            if (string.IsNullOrEmpty(self))
            {
                return string.Empty;
            }

            if (offset < 0)
            {
                return self.Substring(self.Length + offset);
            }

            return self.Substring(offset);
        }

        [NotNull]
        public static string SubstringAfter(
            this string self,
            string token,
            bool include = false,
            CultureInfo culture = null,
            CompareOptions compareOptions = CompareOptions.Ordinal)
        {
            return SubstringAfterInternal(false, self, token, include, culture, compareOptions);
        }

        [NotNull]
        public static string SubstringAfter(
            this string self,
            string token,
            out string rest,
            bool include = false,
            CultureInfo culture = null,
            CompareOptions compareOptions = CompareOptions.Ordinal)
        {
            return SubstringAfterInternal(false, self, token, out rest, include, culture, compareOptions);
        }

        [NotNull]
        public static string SubstringAfter(
            this string self,
            char token,
            bool include = false,
            CultureInfo culture = null,
            CompareOptions compareOptions = CompareOptions.Ordinal)
        {
            return SubstringAfterInternal(false, self, token, include, culture, compareOptions);
        }

        [NotNull]
        public static string SubstringAfter(
            this string self,
            char token,
            out string rest,
            bool include = false,
            CultureInfo culture = null,
            CompareOptions compareOptions = CompareOptions.Ordinal)
        {
            return SubstringAfterInternal(false, self, token, out rest, include, culture, compareOptions);
        }

        [NotNull]
        private static string SubstringAfterInternal(
            bool last,
            string self,
            char token,
            out string rest,
            bool include,
            CultureInfo culture,
            CompareOptions compareOptions)
        {
            rest = string.Empty;
            if (string.IsNullOrEmpty(self))
            {
                return string.Empty;
            }

            if (null == culture)
            {
                culture = CultureInfo.InvariantCulture;
            }

            var compareInfo = culture.CompareInfo;
            var index = last
                ? compareInfo.LastIndexOf(self, token, compareOptions)
                : compareInfo.IndexOf(self, token, compareOptions);

            if (index < 0)
            {
                rest = self;
                return string.Empty;
            }

            rest = self.Substring(0, index);
            if (!include)
            {
                index++;
            }

            return self.Substring(index);
        }

        [NotNull]
        private static string SubstringAfterInternal(
            bool last,
            string self,
            char token,
            bool include,
            CultureInfo culture,
            CompareOptions compareOptions)
        {
            if (string.IsNullOrEmpty(self))
            {
                return string.Empty;
            }

            if (null == culture)
            {
                culture = CultureInfo.InvariantCulture;
            }

            var compareInfo = culture.CompareInfo;
            var index = last
                ? compareInfo.LastIndexOf(self, token, compareOptions)
                : compareInfo.IndexOf(self, token, compareOptions);

            if (index < 0)
            {
                return string.Empty;
            }

            if (!include)
            {
                index++;
            }

            return self.Substring(index);
        }

        [NotNull]
        private static string SubstringAfterInternal(
            bool last,
            string self,
            string token,
            bool include,
            CultureInfo culture,
            CompareOptions compareOptions)
        {
            if (string.IsNullOrEmpty(self) ||
                string.IsNullOrEmpty(token))
            {
                return string.Empty;
            }

            if (null == culture)
            {
                culture = CultureInfo.InvariantCulture;
            }

            var index = last
                ? culture.CompareInfo.LastIndexOf(self, token, compareOptions)
                : culture.CompareInfo.IndexOf(self, token, compareOptions);

            if (index < 0)
            {
                return string.Empty;
            }

            if (!include)
            {
                index += token.Length;
            }

            return self.Substring(index);
        }

        [NotNull]
        private static string SubstringAfterInternal(
            bool last,
            string self,
            string token,
            out string rest,
            bool include,
            CultureInfo culture,
            CompareOptions compareOptions)
        {
            if (string.IsNullOrEmpty(token))
            {
                rest = self;
                return string.Empty;
            }

            if (null == culture)
            {
                culture = CultureInfo.InvariantCulture;
            }

            var compareInfo = culture.CompareInfo;
            var index = last
                ? compareInfo.LastIndexOf(self, token, CompareOptions.Ordinal)
                : compareInfo.IndexOf(self, token, CompareOptions.Ordinal);

            if (index < 0)
            {
                rest = self;
                return string.Empty;
            }

            rest = self.Substring(0, index);
            if (!include)
            {
                index += token.Length;
            }

            return self.Substring(index);
        }

        [NotNull]
        public static string SubstringAfterLast(
            this string self,
            string token,
            bool include = false,
            CultureInfo culture = null,
            CompareOptions compareOptions = CompareOptions.Ordinal)
        {
            return SubstringAfterInternal(true, self, token, include, culture, compareOptions);
        }

        [NotNull]
        public static string SubstringAfterLast(
            this string self,
            string token,
            out string rest,
            bool include = false,
            CultureInfo culture = null,
            CompareOptions compareOptions = CompareOptions.Ordinal)
        {
            return SubstringAfterInternal(true, self, token, out rest, include, culture, compareOptions);
        }

        [NotNull]
        public static string SubstringAfterLast(
            this string self,
            char token,
            bool include = false,
            CultureInfo culture = null,
            CompareOptions compareOptions = CompareOptions.Ordinal)
        {
            return SubstringAfterInternal(true, self, token, include, culture, compareOptions);
        }

        [NotNull]
        public static string SubstringAfterLast(
            this string self,
            char token,
            out string rest,
            bool include = false,
            CultureInfo culture = null,
            CompareOptions compareOptions = CompareOptions.Ordinal)
        {
            return SubstringAfterInternal(true, self, token, out rest, include, culture, compareOptions);
        }

        [NotNull]
        public static string SubstringAfterLastOrSelf(
            this string self,
            string token,
            bool include = false,
            CultureInfo culture = null,
            CompareOptions compareOptions = CompareOptions.Ordinal)
        {
            var result = SubstringAfterInternal(true, self, token, include, culture, compareOptions);
            return result == string.Empty ? self : result;
        }

        [NotNull]
        public static string SubstringAfterLastOrSelf(
            this string self,
            char token,
            bool include = false,
            CultureInfo culture = null,
            CompareOptions compareOptions = CompareOptions.Ordinal)
        {
            var result = SubstringAfterInternal(true, self, token, include, culture, compareOptions);
            return result == string.Empty ? self : result;
        }

        [NotNull]
        public static string SubstringAfterLastOrSelf(
            this string self,
            char token,
            out string rest,
            bool include = false,
            CultureInfo culture = null,
            CompareOptions compareOptions = CompareOptions.Ordinal)
        {
            var result = SubstringAfterInternal(true, self, token, out rest, include, culture, compareOptions);
            if (result == string.Empty)
            {
                rest = string.Empty;
                return self;
            }

            return result;
        }

        [NotNull]
        public static string SubstringAfterLastOrSelf(
            this string self,
            string token,
            out string rest,
            bool include = false,
            CultureInfo culture = null,
            CompareOptions compareOptions = CompareOptions.Ordinal)
        {
            var result = SubstringAfterInternal(true, self, token, out rest, include, culture, compareOptions);
            if (result == string.Empty)
            {
                rest = string.Empty;
                return self;
            }

            return result;
        }

        [NotNull]
        public static string SubstringAfterOrSelf(
            this string self,
            string token,
            bool include = false,
            CultureInfo culture = null,
            CompareOptions compareOptions = CompareOptions.Ordinal)
        {
            var result = SubstringAfterInternal(false, self, token, include, culture, compareOptions);
            return result == string.Empty ? self : result;
        }

        [NotNull]
        public static string SubstringAfterOrSelf(
            this string self,
            string token,
            out string rest,
            bool include = false,
            CultureInfo culture = null,
            CompareOptions compareOptions = CompareOptions.Ordinal)
        {
            var result = SubstringAfterInternal(false, self, token, out rest, include, culture, compareOptions);
            if (result == string.Empty)
            {
                rest = string.Empty;
                return self;
            }

            return result;
        }

        [NotNull]
        public static string SubstringAfterOrSelf(
            this string self,
            char token,
            bool include = false,
            CultureInfo culture = null,
            CompareOptions compareOptions = CompareOptions.Ordinal)
        {
            var result = SubstringAfterInternal(false, self, token, include, culture, compareOptions);
            return result == string.Empty ? self : result;
        }

        [NotNull]
        public static string SubstringAfterOrSelf(
            this string self,
            char token,
            out string rest,
            bool include = false,
            CultureInfo culture = null,
            CompareOptions compareOptions = CompareOptions.Ordinal)
        {
            var result = SubstringAfterInternal(false, self, token, out rest, include, culture, compareOptions);
            if (result == string.Empty)
            {
                rest = string.Empty;
                return self;
            }

            return result;
        }

        [NotNull]
        public static string SubstringBefore(
            this string self,
            string token,
            bool include = false,
            CultureInfo culture = null,
            CompareOptions compareOptions = CompareOptions.Ordinal)
        {
            return SubstringBeforeInternal(false, self, token, include, culture, compareOptions);
        }

        [NotNull]
        public static string SubstringBefore(
            this string self,
            string token,
            out string rest,
            bool include = false,
            CultureInfo culture = null,
            CompareOptions compareOptions = CompareOptions.Ordinal)
        {
            return SubstringBeforeInternal(false, self, token, out rest, include, culture, compareOptions);
        }

        [NotNull]
        public static string SubstringBefore(
            this string self,
            char token,
            bool include = false,
            CultureInfo culture = null,
            CompareOptions compareOptions = CompareOptions.Ordinal)
        {
            return SubstringBeforeInternal(false, self, token, include, culture, compareOptions);
        }

        [NotNull]
        public static string SubstringBefore(
            this string self,
            char token,
            out string rest,
            bool include = false,
            CultureInfo culture = null,
            CompareOptions compareOptions = CompareOptions.Ordinal)
        {
            return SubstringBeforeInternal(false, self, token, out rest, include, culture, compareOptions);
        }

        [NotNull]
        private static string SubstringBeforeInternal(
            bool last,
            string self,
            string token,
            bool include,
            CultureInfo culture,
            CompareOptions compareOptions)
        {
            if (string.IsNullOrEmpty(token))
            {
                return string.Empty;
            }

            if (null == culture)
            {
                culture = CultureInfo.InvariantCulture;
            }

            var compareInfo = culture.CompareInfo;
            var index = last
                ? compareInfo.LastIndexOf(self, token, compareOptions)
                : compareInfo.IndexOf(self, token, compareOptions);

            if (index < 0)
            {
                return string.Empty;
            }

            if (include)
            {
                index += token.Length;
            }

            return self.Substring(0, index);
        }

        [NotNull]
        private static string SubstringBeforeInternal(
            bool last,
            string self,
            char token,
            bool include,
            CultureInfo culture,
            CompareOptions compareOptions)
        {
            if (string.IsNullOrEmpty(self))
            {
                return string.Empty;
            }

            if (null == culture)
            {
                culture = CultureInfo.InvariantCulture;
            }

            var compareInfo = culture.CompareInfo;
            var index = last
                ? compareInfo.LastIndexOf(self, token, compareOptions)
                : compareInfo.IndexOf(self, token, compareOptions);

            if (index < 0)
            {
                // No such substring
                return string.Empty;
            }

            if (include)
            {
                index++;
            }

            return self.Substring(0, index);
        }

        [NotNull]
        private static string SubstringBeforeInternal(
            bool last,
            string self,
            char token,
            out string rest,
            bool include,
            CultureInfo culture,
            CompareOptions compareOptions)
        {
            rest = string.Empty;
            if (string.IsNullOrEmpty(self))
            {
                return string.Empty;
            }

            if (null == culture)
            {
                culture = CultureInfo.InvariantCulture;
            }

            var compareInfo = culture.CompareInfo;
            var index = last
                ? compareInfo.LastIndexOf(self, token, compareOptions)
                : compareInfo.IndexOf(self, token, compareOptions);

            if (index < 0)
            {
                rest = self;
                return string.Empty;
            }

            var restLength = index + 1;
            if (include)
            {
                index = restLength;
            }

            rest = self.Substring(restLength);
            return self.Substring(0, index);
        }

        [NotNull]
        private static string SubstringBeforeInternal(
            bool last,
            string self,
            string token,
            out string rest,
            bool include,
            CultureInfo culture,
            CompareOptions compareOptions)
        {
            if (string.IsNullOrEmpty(token))
            {
                rest = self;
                return string.Empty;
            }

            if (null == culture)
            {
                culture = CultureInfo.InvariantCulture;
            }

            var compareInfo = culture.CompareInfo;
            var index = last
                ? compareInfo.LastIndexOf(self, token, compareOptions)
                : compareInfo.IndexOf(self, token, compareOptions);

            if (index < 0)
            {
                rest = self;
                return string.Empty;
            }

            var restIndex = index + token.Length;
            if (include)
            {
                index = restIndex;
            }

            rest = self.Substring(restIndex);
            return self.Substring(0, index);
        }

        [NotNull]
        public static string SubstringBeforeLast(
            this string self,
            string token,
            bool include = false,
            CultureInfo culture = null,
            CompareOptions compareOptions = CompareOptions.Ordinal)
        {
            return SubstringBeforeInternal(true, self, token, include, culture, compareOptions);
        }

        [NotNull]
        public static string SubstringBeforeLast(
            this string self,
            string token,
            out string rest,
            bool include = false,
            CultureInfo culture = null,
            CompareOptions compareOptions = CompareOptions.Ordinal)
        {
            return SubstringBeforeInternal(true, self, token, out rest, include, culture, compareOptions);
        }

        [NotNull]
        public static string SubstringBeforeLast(
            this string self,
            char token,
            bool include = false,
            CultureInfo culture = null,
            CompareOptions compareOptions = CompareOptions.Ordinal)
        {
            return SubstringBeforeInternal(true, self, token, include, culture, compareOptions);
        }

        [NotNull]
        public static string SubstringBeforeLast(
            this string self,
            char token,
            out string rest,
            bool include = false,
            CultureInfo culture = null,
            CompareOptions compareOptions = CompareOptions.Ordinal)
        {
            return SubstringBeforeInternal(true, self, token, out rest, include, culture, compareOptions);
        }

        [NotNull]
        public static string SubstringBeforeLastOrSelf(
            this string self,
            string token,
            bool include = false,
            CultureInfo culture = null,
            CompareOptions compareOptions = CompareOptions.Ordinal)
        {
            var result = SubstringBeforeInternal(true, self, token, include, culture, compareOptions);
            return result == string.Empty ? self : result;
        }

        [NotNull]
        public static string SubstringBeforeLastOrSelf(
            this string self,
            string token,
            out string rest,
            bool include = false,
            CultureInfo culture = null,
            CompareOptions compareOptions = CompareOptions.Ordinal)
        {
            var result = SubstringBeforeInternal(true, self, token, out rest, include, culture, compareOptions);
            if (result == string.Empty)
            {
                rest = string.Empty;
                return self;
            }

            return result;
        }

        [NotNull]
        public static string SubstringBeforeLastOrSelf(
            this string self,
            char token,
            bool include = false,
            CultureInfo culture = null,
            CompareOptions compareOptions = CompareOptions.Ordinal)
        {
            var result = SubstringBeforeInternal(true, self, token, include, culture, compareOptions);
            return result == string.Empty ? self : result;
        }

        [NotNull]
        public static string SubstringBeforeLastOrSelf(
            this string self,
            char token,
            out string rest,
            bool include = false,
            CultureInfo culture = null,
            CompareOptions compareOptions = CompareOptions.Ordinal)
        {
            var result = SubstringBeforeInternal(true, self, token, out rest, include, culture, compareOptions);
            if (result == string.Empty)
            {
                rest = string.Empty;
                return self;
            }

            return result;
        }

        [NotNull]
        public static string SubstringBeforeOrSelf(
            this string self,
            string token,
            bool include = false,
            CultureInfo culture = null,
            CompareOptions compareOptions = CompareOptions.Ordinal)
        {
            var result = SubstringBeforeInternal(false, self, token, include, culture, compareOptions);
            return result == string.Empty ? self : result;
        }

        [NotNull]
        public static string SubstringBeforeOrSelf(
            this string self,
            char token,
            bool include = false,
            CultureInfo culture = null,
            CompareOptions compareOptions = CompareOptions.Ordinal)
        {
            var result = SubstringBeforeInternal(false, self, token, include, culture, compareOptions);
            return result == string.Empty ? self : result;
        }

        [NotNull]
        public static string SubstringBeforeOrSelf(
            this string self,
            string token,
            out string rest,
            bool include = false,
            CultureInfo culture = null,
            CompareOptions compareOptions = CompareOptions.Ordinal)
        {
            var result = SubstringBeforeInternal(false, self, token, out rest, include, culture, compareOptions);
            if (result == string.Empty)
            {
                rest = string.Empty;
                return self;
            }

            return result;
        }

        [NotNull]
        public static string SubstringBeforeOrSelf(
            this string self,
            char token,
            out string rest,
            bool include = false,
            CultureInfo culture = null,
            CompareOptions compareOptions = CompareOptions.Ordinal)
        {
            var result = SubstringBeforeInternal(false, self, token, out rest, include, culture, compareOptions);
            if (result == string.Empty)
            {
                rest = string.Empty;
                return self;
            }

            return result;
        }

        [NotNull]
        [Pure]
        public static IndexedCharEnumerator GetIndexedEnumerator(this string self)
        {
            return new IndexedCharEnumerator(self);
        }

        [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
        [System.Runtime.InteropServices.ComVisible(true)]
#if !SILVERLIGHT
        [Serializable]
#endif
        public struct IndexedCharEnumerator : IEnumerator<char>
        {
            private string _source;
            private int _index;
            private char _current;

            internal IndexedCharEnumerator(string source)
            {
                if (source == null)
                    throw new ArgumentNullException(nameof(source));

                _source = source;
                _index = -1;
                _current = '\0';
            }

            public int Index => _index;
            public bool IsStarted => _index != -1;
            public bool IsFinished => _index >= _source.Length;
            public char Current => _current;

            object IEnumerator.Current => _current;

            public bool MoveNext()
            {
                if (_index < (_source.Length - 1))
                {
                    _index++;
                    _current = _source[_index];
                    return true;
                }
                else
                    _index = _source.Length;
                return false;

            }

            public void Reset()
            {
                _current = '\0';
                _index = -1;
            }

            public void Dispose()
            {
                if (_source != null)
                    _index = _source.Length;

                _source = null;
            }

            public override string ToString()
            {
                if (IsStarted)
                {
                    if (!IsFinished)
                    {
                        return Current.ToString(); // TODO ...prefix<Current>suffix...
                    }

                    return "Finished";
                }

                return "Not Started";
            }

            public int MoveTo(ref IndexedCharEnumerator other)
            {
                while (MoveNext())
                {
                    if (other.Current == Current)
                    {
                        return Index;
                    }
                }

                return -1;
            }

            public int MoveWith(ref IndexedCharEnumerator other)
            {
                int start = -1;
                while (MoveNext())
                {
                    if (other.Current != Current)
                        continue;

                    start = Index;
                    while (other.MoveNext() & MoveNext())
                    {
                        if (other.Current != Current)
                        {
                            break;
                        }
                    }

                    if (other.IsFinished || IsFinished)
                    {
                        break;
                    }
                    else
                    {
                        _index = start;
                        start = -1;
                        other.Reset();
                        other.MoveNext();
                    }
                }

                return start;
            }
        }
    }
}
