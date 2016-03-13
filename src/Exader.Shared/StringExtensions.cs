using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using JetBrains.Annotations;

namespace Exader
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public static partial class StringExtensions
    {
#if SILVERLIGHT || NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [NotNull]
        [Pure]
        public static string Capitalize(this string self, CultureInfo culture = null)
        {
            if (string.IsNullOrEmpty(self))
            {
                return string.Empty;
            }

            char upper = char.ToUpper(self[0], culture ?? CultureInfo.InvariantCulture);
            if (self[0] == upper)
            {
                return self;
            }

            return (upper + self.Substring(1));
        }

#if SILVERLIGHT || NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [NotNull]
        [Pure]
        public static string Collapse(this string self, char collapsible, int minWidth = 0)
        {
            if (string.IsNullOrEmpty(self))
            {
                return string.Empty;
            }

            if (minWidth < 0)
            {
                minWidth = 0;
            }

            var buffer = new StringBuilder();
            int width = 0; // Счётчик ширины
            for (int i = 0; i < self.Length; i++)
            {
                char c = self[i];
                if (c == collapsible)
                {
                    width++;
                }
                else
                {
                    width = 0;
                }

                if (width <= minWidth) buffer.Append(c);
            }

            return buffer.ToString();
        }

#if SILVERLIGHT || NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [NotNull]
        [Pure]
        public static string Collapse(this string self, string characterString, int minWidth = 0)
        {
            if (string.IsNullOrEmpty(self)) return string.Empty;
            if (string.IsNullOrEmpty(characterString)) return self;

            if (1 == characterString.Length)
            {
                return Collapse(self, characterString[0], minWidth);
            }

            if (minWidth < 0) minWidth = 0;

            var buffer = new StringBuilder();
            int width = 0; // Счётчик ширины
            for (int i = 0; i < self.Length; i++)
            {
                char c = self[i];
                if (characterString.Contains(c))
                {
                    width++;
                }
                else
                {
                    width = 0;
                }

                if (width <= minWidth)
                {
                    buffer.Append(c);
                }
            }

            return buffer.ToString();
        }

        /// <summary>
        /// Сжимает длину стоки за счёт пробелов.
        /// </summary>
        /// <param name="self"></param>
        /// <param name="minWidth">Минимальная ширина пробела.</param>
        /// <returns></returns>
        [NotNull]
        [Pure]
        public static string CollapseWhiteSpaces(this string self, int minWidth = 0)
        {
            if (string.IsNullOrEmpty(self)) return string.Empty;

            if (minWidth < 0)
            {
                minWidth = 0;
            }

            var buffer = new StringBuilder();
            int width = 0; // Счётчик ширины пробела
            for (int i = 0; i < self.Length; i++)
            {
                char c = self[i];
                if (char.IsWhiteSpace(c))
                {
                    width++;
                }
                else
                {
                    width = 0;
                }

                if (width <= minWidth)
                {
                    buffer.Append(c);
                }
            }

            return buffer.ToString();
        }

        [NotNull]
        [Pure]
        public static string CollapseWhiteSpacesToCamelHumps(this string self, CultureInfo culture = null)
        {
            if (string.IsNullOrEmpty(self)) return string.Empty;

            if (culture == null) culture = CultureInfo.InvariantCulture;

            var buffer = new StringBuilder();
            bool hump = true;
            for (int i = 0; i < self.Length; i++)
            {
                char c = self[i];
                if (char.IsWhiteSpace(c))
                {
                    hump = true;
                }
                else
                {
                    if (hump)
                    {
                        hump = false;

                        buffer.Append(c.ToUpper(culture));
                    }
                    else
                    {
                        buffer.Append(c);
                    }
                }
            }

            return buffer.ToString();
        }
        
        [Pure]
        public static string CommonPrefixWith(this string self, string other)
        {
            if (!string.IsNullOrEmpty(self) && !string.IsNullOrEmpty(other))
            {
                int index;
                int lengh = Math.Min(self.Length, other.Length);
                for (index = 0; index < lengh; index++)
                {
                    if (self[index] != other[index])
                    {
                        break;
                    }
                }

                if (0 < index)
                {
                    return self.Substring(0, index);
                }
            }

            return null;
        }

#if SILVERLIGHT || NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [Pure]
        public static bool Contains(this string self, char c)
        {
            return 0 <= self.IndexOf(c);
        }

#if SILVERLIGHT || NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [Pure]
        public static bool Contains(this string self, string subString, StringComparison comparison)
        {
            return 0 <= self.IndexOf(subString, 0, comparison);
        }
        
        [Pure]
        public static bool ContainsAll(this string self, params char[] chars)
        {
            for (int i = 0; i < chars.Length; i++)
            {
                if (-1 == self.IndexOf(chars[i]))
                {
                    return false;
                }
            }

            return true;
        }
        
        [Pure]
        public static bool ContainsAny(this string self, params char[] chars)
        {
            for (int i = 0; i < chars.Length; i++)
            {
                if (0 <= self.IndexOf(chars[i]))
                {
                    return true;
                }
            }

            return false;
        }

#if !NET35
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [Pure]
        public static bool ContainsIgnoreCase(this string self, string subString)
        {
            return 0 <= self.IndexOf(subString, 0, StringComparison.InvariantCultureIgnoreCase);
        }
        
        [Pure]
        public static int Count(this string self, char ch)
        {
            if (self == null)
            {
                return 0;
            }

            int count = 0;
            foreach (char c in self)
            {
                if (c == ch)
                {
                    count++;
                }
            }

            return count;
        }

        [NotNull]
        [Pure]
        public static string Decapitalize(this string self, CultureInfo culture = null)
        {
            if (string.IsNullOrEmpty(self))
            {
                return string.Empty;
            }

            char lower = char.ToLower(self[0], culture ?? CultureInfo.InvariantCulture);
            if (self[0] == lower)
            {
                return self;
            }

            return (lower + self.Substring(1));
        }

#if SILVERLIGHT || NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [Pure]
        public static string Deparenthesize(this string self, char open = '(', char close = ')')
        {
            return Unquote(self, open, close);
        }
        
        [Pure]
        public static string Ellipsis(this string self, int maxLength, params char[] stops)
        {
            if (string.IsNullOrEmpty(self) || self.Length < maxLength)
            {
                return self;
            }

            if (null == stops || 0 == stops.Length)
            {
                if (string.IsNullOrEmpty(self) || self.Length < maxLength)
                {
                    return self;
                }

                string result = self.Substring(0, maxLength / 2) + "…" + self.Right((maxLength / 2) - 1);
                return result;
            }
            else
            {
                int mid = (maxLength / 2);
                if (1 == stops.Length)
                {
                    char stop = stops[0];
                    for (int index = mid; 0 < index; index--)
                    {
                        if (stop == self[index])
                        {
                            mid = index;
                            break;
                        }
                    }
                }
                else
                {
                    for (int index = mid; 0 < index; index--)
                    {
                        if (stops.Contains(self[index]))
                        {
                            mid = index;
                            break;
                        }
                    }
                }

                string result = self.Substring(0, mid + 1) + "…" + self.Right(maxLength - mid - 2);
                return result;
            }
        }

#if SILVERLIGHT || NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [Pure]
        public static bool EndsWith(this string self, char end)
        {
            if (string.IsNullOrEmpty(self)) return false;

            return end == self[self.Length - 1];
        }

#if SILVERLIGHT || NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [Pure]
        public static bool EndsWithIgnoreCase(this string self, string end)
        {
            if (null == self) return false;

            return self.EndsWith(end, StringComparison.InvariantCultureIgnoreCase);
        }

#if SILVERLIGHT || NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [Pure]
        public static string EnsureEndsWith(this string self, char end)
        {
            if (!string.IsNullOrEmpty(self))
            {
                if (end != self[self.Length - 1])
                {
                    return self + end;
                }

                return self;
            }

            return end.ToString(CultureInfo.InvariantCulture);
        }

#if SILVERLIGHT || NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [Pure]
        public static string EnsureEndsWith(this string self, string end)
        {
            // AND
            if (string.IsNullOrEmpty(self) && string.IsNullOrEmpty(end))
            {
                return string.Empty;
            }

            // XOR
            if (string.IsNullOrEmpty(self)) return end;
            if (string.IsNullOrEmpty(end)) return self;

            return self.EndsWith(end) ? self : self + end;
        }

#if SILVERLIGHT || NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [Pure]
        public static string EnsureEndsWith(this string self, string end, StringComparison stringComparison)
        {
            // AND
            if (string.IsNullOrEmpty(self) && string.IsNullOrEmpty(end))
            {
                return string.Empty;
            }

            // XOR
            if (string.IsNullOrEmpty(self)) return end;
            if (string.IsNullOrEmpty(end)) return self;

            return self.EndsWith(end, stringComparison) ? self : self + end;
        }

#if SILVERLIGHT || NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [Pure]
        public static string EnsureStartsWith(this string self, char start)
        {
            if (!string.IsNullOrEmpty(self))
            {
                if (start != self[0])
                {
                    return start + self;
                }

                return self;
            }

            return start.ToString(CultureInfo.InvariantCulture);
        }

#if SILVERLIGHT || NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [Pure]
        public static string EnsureStartsWith(this string self, string start)
        {
            // AND
            if (string.IsNullOrEmpty(self) && string.IsNullOrEmpty(start))
            {
                return string.Empty;
            }

            // XOR
            if (string.IsNullOrEmpty(self)) return start;
            if (string.IsNullOrEmpty(start)) return self;

            return self.StartsWith(start) ? self : start + self;
        }

#if SILVERLIGHT || NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [Pure]
        public static string EnsureStartsWith(this string self, string start, StringComparison stringComparison)
        {
            // AND
            if (string.IsNullOrEmpty(self) && string.IsNullOrEmpty(start))
            {
                return string.Empty;
            }

            // XOR
            if (string.IsNullOrEmpty(self)) return start;
            if (string.IsNullOrEmpty(start)) return self;

            return self.StartsWith(start, stringComparison) ? self : start + self;
        }

#if SILVERLIGHT || NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [Pure]
        public static bool EqualsIgnoreCase(this string self, string other)
        {
            return (string.Compare(self, other, CultureInfo.InvariantCulture, CompareOptions.IgnoreCase) == 0);
        }

#if SILVERLIGHT || NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [Pure]
        public static string Escape(this string self)
        {
            return self.Replace(@"\", @"\\").Replace("\"", "\\\"").Replace("'", @"\'");
        }

        /// <summary>
        /// Добавляет пробелы перед символами в большом регистре после малого.
        /// </summary>
        /// <param name="self"></param>
        /// <param name="delimeter"></param>
        /// <returns></returns>
        [NotNull]
        [Pure]
        public static string ExpandCamelHumps(this string self, string delimeter = " ")
        {
            if (string.IsNullOrEmpty(self)) return string.Empty;

            var buffer = new StringBuilder();
            foreach (char c in self)
            {
                if (char.IsUpper(c) && (0 < buffer.Length))
                {
                    buffer.Append(delimeter);
                }

                buffer.Append(c);
            }

            return buffer.ToString();
        }
        
        [Pure]
        public static string ExpandTabs(this string self, byte tabSize)
        {
            var buffer = new StringBuilder();
            for (int i = 0; i < self.Length; i++)
            {
                char c = self[i];
                if ('\t' == c)
                {
                    buffer.Append(' ', tabSize);
                }
                else
                {
                    buffer.Append(c);
                }
            }

            return buffer.ToString();
        }
        
        [Pure]
        public static string Indent(this string self, int level)
        {
            return InsertLinePrefix(self, "\t".PadLeft(level, '\t'));
        }
        
        [Pure]
        public static string Indent(this string self)
        {
            return InsertLinePrefix(self, "\t");
        }

#if SILVERLIGHT || NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [Pure]
        public static int IndexOfIgnoreCase(this string self, string other, int startIndex = 0, int count = int.MaxValue)
        {
            if (null == self)
            {
                return -1;
            }

            if (count == int.MaxValue)
            {
                count = self.Length - startIndex;
            }

            return self.IndexOf(other, startIndex, count, StringComparison.InvariantCultureIgnoreCase);
        }

        public static string InsertLinePrefix(this string self, string prefix)
        {
            return InsertLinePrefix(self, prefix, '\n');
        }

        public static string InsertLinePrefix(this string self, string prefix, char delimiter)
        {
            var buffer = new StringBuilder();
            string[] lines = self /*.Trim(delimiter)*/.Split(delimiter);
            ////if (self.StartsWith(delimiter.ToString()))
            ////{
            ////    buffer.Append(delimiter);
            ////}

            bool tail = false;
            foreach (string line in lines)
            {
                if (tail)
                {
                    buffer.Append(delimiter);
                }
                tail = true;

                buffer.Append(prefix);
                buffer.Append(line);
            }

            ////if (self.EndsWith(delimiter.ToString()))
            ////{
            ////    buffer.Append(delimiter);
            ////}

            return buffer.ToString();
        }

#if SILVERLIGHT || NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool JoinsWith(this string self, char c)
        {
            var index = self.IndexOf(c);
            return 0 < index && index < self.Length - 1;
        }

#if SILVERLIGHT || NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool JoinsWith(this string self, string subString, StringComparison comparison)
        {
            var index = self.IndexOf(subString, 0, comparison);
            return 0 < index && index < self.Length - 1;
        }

#if SILVERLIGHT || NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool JoinsWithIgnoreCase(this string self, string subString)
        {
            var index = self.IndexOf(subString, 0, StringComparison.InvariantCultureIgnoreCase);
            return 0 < index && index < self.Length - 1;
        }

#if SILVERLIGHT || NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int LastIndexOfIgnoreCase(this string self, string other, int startIndex = int.MaxValue, int count = int.MaxValue)
        {
            if (null == self) return -1;

            if (startIndex == int.MaxValue)
            {
                startIndex = self.Length - 1;
            }
            else if (startIndex < 0)
            {
                startIndex = self.Length + startIndex - 1;
            }

            if (count == int.MaxValue) count = startIndex + 1;

            return self.LastIndexOf(other, startIndex, count, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Возвращает начало строки заданной длины.
        /// </summary>
        /// <param name="self"></param>
        /// <param name="length">Длина может привышать фактическую или задана от конца отрицательным смещением.</param>
        /// <returns></returns>
#if SILVERLIGHT || NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string Left([NotNull] this string self, int length)
        {
            if (length < 0)
            {
                length = self.Length + length;
                if (length < 0) return self;
            }

            if (self.Length <= length)
            {
                return self;
            }

            if (0 == length)
            {
                return string.Empty;
            }

            return self.Substring(0, length);
        }

#if SILVERLIGHT || NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string Parenthesize(this string self, char open = '(', char close = ')', bool valuable = false)
        {
            return Quote(self, open, close, valuable);
        }

        /// <summary>
        /// Добавляет пробелы перед символами в большом регистре после малого.
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        [NotNull]
        [Obsolete("Use ExpandCamelHumps()")]
        public static string PutWhiteSpaces(this string self)
        {
            return ExpandCamelHumps(self);
        }

#if SILVERLIGHT || NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string Quote(this string self, char mark, bool valuable = false)
        {
            if (!string.IsNullOrEmpty(self))
            {
                if (mark != self[0])
                {
                    self = mark + self;
                }

                if (mark != self[self.Length - 1])
                {
                    self = self + mark;
                }

                return self;
            }

            return valuable
                ? mark.ToString(CultureInfo.InvariantCulture) + mark
                : string.Empty;
        }

#if SILVERLIGHT || NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string Quote(this string self, char open, char close, bool valuable = false)
        {
            if (!string.IsNullOrEmpty(self))
            {
                if (open != self[0])
                {
                    self = open + self;
                }

                if (close != self[self.Length - 1])
                {
                    self = self + close;
                }

                return self;
            }

            return valuable
                ? open.ToString(CultureInfo.InvariantCulture) + close
                : string.Empty;
        }

#if SILVERLIGHT || NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string Quote(this string self, string mark, bool valuable = false)
        {
            // AND
            if (string.IsNullOrEmpty(self) && string.IsNullOrEmpty(mark))
            {
                return string.Empty;
            }

            // XOR
            if (string.IsNullOrEmpty(self))
            {
                return valuable
                    ? mark + mark
                    : string.Empty;
            }

            if (string.IsNullOrEmpty(mark))
            {
                return self;
            }

            if (!self.StartsWith(mark))
            {
                self = mark + self;
            }

            if (!self.EndsWith(mark))
            {
                self = self + mark;
            }

            return self;
        }

#if SILVERLIGHT || NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string Quote(this string self, string open, string close)
        {
            // AND
            if (string.IsNullOrEmpty(self) &&
                string.IsNullOrEmpty(open) &&
                string.IsNullOrEmpty(close))
            {
                return string.Empty;
            }

            // XOR
            if (string.IsNullOrEmpty(self))
            {
                return open + close;
            }

            if (string.IsNullOrEmpty(open) ||
                string.IsNullOrEmpty(close))
            {
                return self;
            }

            if (!self.StartsWith(open))
            {
                self = open + self;
            }

            if (!self.EndsWith(close))
            {
                self = self + close;
            }

            return self;
        }

        [NotNull]
        public static string RemoveCharacter(this string self, params char[] charsToRemove)
        {
            if (string.IsNullOrEmpty(self))
            {
                return string.Empty;
            }

            var builder = new StringBuilder(self.Length);
            foreach (char c in self)
            {
                if (-1 == Array.IndexOf(charsToRemove, c))
                {
                    builder.Append(c);
                }
            }

            return builder.ToString();
        }

        [NotNull]
        public static string RemoveCharacter(this string self, char charToRemove)
        {
            if (string.IsNullOrEmpty(self)) return string.Empty;

            var builder = new StringBuilder(self.Length);
            foreach (char c in self)
            {
                if (c != charToRemove)
                {
                    builder.Append(c);
                }
            }

            return builder.ToString();
        }

        [Obsolete("Use RemoveCharacter method instead")]
        [NotNull]
        public static string RemoveChars(this string self, params char[] charsToRemove)
        {
            return RemoveCharacter(self, charsToRemove);
        }

        [Obsolete("Use RemoveCharacter method instead")]
        [NotNull]
        public static string RemoveChars(this string self, char charToRemove)
        {
            return RemoveCharacter(self, charToRemove);
        }

        [Obsolete("Use ReplaceControlCharacters method instead")]
        public static string RemoveControlChars(this string self, string replacement = "")
        {
            return ReplaceControlCharacters(self, replacement);
        }

#if SILVERLIGHT || NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string RemoveLeft([NotNull] this string self, int count)
        {
            if (self.Length <= count)
            {
                return string.Empty;
            }

            if (0 == count)
            {
                return self;
            }

            return self.Substring(count);
        }

        public static string RemoveLinePrefix(this string self, string prefix)
        {
            return RemoveLinePrefix(self, prefix, '\n');
        }

        public static string RemoveLinePrefix(this string self, string prefix, char delimiter)
        {
            var buffer = new StringBuilder();
            string[] lines = self.Split(delimiter);

            bool tail = false;
            foreach (string line in lines)
            {
                if (tail)
                {
                    buffer.Append(delimiter);
                }

                tail = true;

                buffer.Append(line.StartsWith(prefix) ? line.Remove(0, prefix.Length) : line);
            }

            return buffer.ToString();
        }

#if SILVERLIGHT || NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string RemoveRight([NotNull] this string self, int count)
        {
            if (self.Length <= count)
            {
                return string.Empty;
            }

            if (0 == count)
            {
                return self;
            }

            return self.Substring(0, self.Length - count);
        }

#if SILVERLIGHT || NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string RemoveSubstring(this string self, string str)
        {
            if (string.IsNullOrEmpty(self))
            {
                return string.Empty;
            }

            return self.Replace(str, string.Empty);
        }

#if SILVERLIGHT || NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string Repeat(this string self, int count)
        {
            if (count == 0) return string.Empty;

            StringBuilder buffer = self.ToBuilder();
            while (0 < --count)
            {
                buffer.Append(self);
            }

            return buffer.ToString();
        }

#if SILVERLIGHT || NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string Repeat(this string self)
        {
            return self + self;
        }

        public static string Replace(this string self, Func<char, char> replacer)
        {
            var result = new StringBuilder(self.Length);
            foreach (char c in self)
            {
                result.Append(replacer(c));
            }

            return result.ToString();
        }

        /// <summary>
        /// Ищет символы из заданного набора и заменяет на строку.
        /// </summary>
        /// <param name="self"></param>
        /// <param name="replacement">Строка замена</param>
        /// <param name="characterString">искомый набор символов для замены</param>
        /// <returns></returns>
        [NotNull]
        public static string ReplaceCharacters(this string self, string replacement, string characterString)
        {
            if (string.IsNullOrEmpty(self))
            {
                return string.Empty;
            }

            if (string.IsNullOrEmpty(characterString))
            {
                return self;
            }

            var builder = new StringBuilder(self.Length);
            foreach (char c in self)
            {
                if (characterString.Contains(c))
                {
                    builder.Append(replacement);
                }
                else
                {
                    builder.Append(c);
                }
            }

            return builder.ToString();
        }

        [NotNull]
        public static string ReplaceCharacters(this string self, string replacement, params char[] charsToRemove)
        {
            if (string.IsNullOrEmpty(self))
            {
                return string.Empty;
            }

            if (charsToRemove.IsNullOrEmpty())
            {
                return self;
            }

            var builder = new StringBuilder(self.Length);
            foreach (char c in self)
            {
                if (-1 == Array.IndexOf(charsToRemove, c))
                {
                    builder.Append(c);
                }
                else
                {
                    builder.Append(replacement);
                }
            }

            return builder.ToString();
        }

        /// <summary>
        /// Удаляет из символы управления из текста.
        /// </summary>
        /// <param name="self"></param>
        /// <param name="replacement"></param>
        /// <returns></returns>
        public static string ReplaceControlCharacters(this string self, string replacement = "")
        {
            if (string.IsNullOrEmpty(self))
            {
                return string.Empty;
            }

            var buffer = new StringBuilder();
            foreach (char c in self)
            {
                switch (char.GetUnicodeCategory(c))
                {
                    case UnicodeCategory.Control:
                    case UnicodeCategory.Format:
                    case UnicodeCategory.Surrogate:
                    case UnicodeCategory.PrivateUse:
                    case UnicodeCategory.OtherNotAssigned:
                        buffer.Append(replacement);
                        break;
                    default:
                        buffer.Append(c);
                        break;
                }
            }

            return buffer.ToString();
        }

        /// <summary>
        /// Возвращает окончание строки заданной длины.
        /// </summary>
        /// <param name="self"></param>
        /// <param name="length">Длина может привышать фактическую или задана от начала отрицательным смещением.</param>
        /// <returns></returns>
#if SILVERLIGHT || NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string Right(this string self, int length)
        {
            int startIndex;
            if (length < 0)
            {
                startIndex = -1 * length;
                length = self.Length + length;
                if (length < 0) return self;
            }
            else
            {
                startIndex = self.Length - length;
            }

            if (self.Length <= length)
            {
                return self;
            }

            if (0 == length)
            {
                return string.Empty;
            }

            return self.Substring(startIndex, length);
        }

        public static string[] SplitAndRemoveEmpties(this string self, params char[] separators)
        {
            return self.Split(separators, StringSplitOptions.RemoveEmptyEntries);
        }

        public static string[] SplitAndRemoveEmpties([NotNull] this string self, params string[] separators)
        {
            return self.Split(separators, StringSplitOptions.RemoveEmptyEntries);
        }

#if SILVERLIGHT || NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool StartsWith(this string self, char start)
        {
            if (string.IsNullOrEmpty(self))
            {
                return false;
            }

            return start == self[0];
        }

#if SILVERLIGHT || NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool StartsWithIgnoreCase(this string self, string start)
        {
            if (null == self)
            {
                return false;
            }

            return self.StartsWith(start, StringComparison.InvariantCultureIgnoreCase);
        }

#if SILVERLIGHT || NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [NotNull]
        public static string TrimEnd(this string self, string end)
        {
            if (string.IsNullOrEmpty(self) || string.IsNullOrEmpty(end))
            {
                return string.Empty;
            }

            if (!self.EndsWith(end))
            {
                return self;
            }

            return self.Substring(0, self.Length - end.Length);
        }

#if SILVERLIGHT || NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [CanBeNull]
        [Pure]
        public static string TrimEndOrNull(this string self)
        {
            if (!string.IsNullOrEmpty(self))
            {
                self = self.TrimEnd();
                return string.Empty != self ? self : null;
            }

            return null;
        }

#if SILVERLIGHT || NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [CanBeNull]
        [Pure]
        public static string TrimEndOrNull(this string self, string end)
        {
            self = TrimEnd(self, end);
            return string.Empty != self ? self : null;
        }

#if SILVERLIGHT || NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [NotNull]
        [Pure]
        public static string TrimExceed(this string self, int maxLength)
        {
            if (string.IsNullOrEmpty(self))
            {
                return string.Empty;
            }

            if (self.Length < maxLength)
            {
                return self;
            }

            string result = self.Substring(0, maxLength);
            return result;
        }

#if SILVERLIGHT || NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [CanBeNull]
        public static string TrimOrNull(this string self)
        {
            if (!string.IsNullOrEmpty(self))
            {
                self = self.Trim();
                return string.Empty != self ? self : null;
            }

            return null;
        }

#if SILVERLIGHT || NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [NotNull]
        public static string TrimStart(this string self, string start)
        {
            if (string.IsNullOrEmpty(self)) return string.Empty;

            if (!self.StartsWith(start))
            {
                return self;
            }

            return self.Substring(start.Length);
        }

#if SILVERLIGHT || NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [CanBeNull]
        [Pure]
        public static string TrimStartOrNull(this string self)
        {
            if (!string.IsNullOrEmpty(self))
            {
                self = self.TrimStart();
                return string.Empty != self ? self : null;
            }

            return null;
        }

#if SILVERLIGHT || NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [CanBeNull]
        [Pure]
        public static string TrimStartOrNull(this string self, string start)
        {
            self = TrimStart(self, start);
            return string.Empty != self ? self : null;
        }

        public static string Unindent(this string self, int level = 1, char indent = '\t', int indentSize = 1)
        {
            var prefix = indent.ToString(CultureInfo.InvariantCulture).PadLeft(level * indentSize, indent);
            return RemoveLinePrefix(self, prefix);
        }

        public static string Unquote(this string self)
        {
            if (2 <= self.Length)
            {
                char f = self[0];
                char l = self[self.Length - 1];
                if (f.IsQuotationMarks(l))
                {
                    return self.Substring(1, self.Length - 2);
                }
            }

            return self;
        }

        [NotNull]
        public static string Unquote(this string self, char open, char close)
        {
            if (string.IsNullOrEmpty(self))
            {
                return string.Empty;
            }

            if (1 == self.Length)
            {
                return self;
            }

            int oi = 0;
            bool opened = false;
            while (oi < self.Length)
            {
                if (self[oi] == open)
                {
                    oi++;
                    opened = true;
                    break;
                }

                oi++;
            }

            int ci = self.Length - 1;
            bool closed = false;
            while (oi < ci)
            {
                if (self[ci] == close)
                {
                    closed = true;
                    break;
                }

                ci--;
            }

            if (opened && closed)
            {
                return self.Substring(oi, ci - oi);
            }

            return self;
        }

        [NotNull]
        public static string Unrepeat(this string self, params char[] charsToRemove)
        {
            if (string.IsNullOrEmpty(self))
            {
                return string.Empty;
            }

            if (1 == self.Length)
            {
                return self;
            }

            var builder = new StringBuilder(self.Length).Append(self[0]);

            char prevChar = self[0];
            for (int i = 1; i < self.Length; i++)
            {
                char c = self[i];
                if (charsToRemove.Contains(c))
                {
                    if (prevChar != c)
                    {
                        builder.Append(c);
                    }
                }
                else
                {
                    builder.Append(c);
                }

                prevChar = c;
            }

            return builder.ToString();
        }

        public static string Unwrap(this string self, bool collapse = false)
        {
            if (string.IsNullOrEmpty(self))
            {
                return self;
            }

            int repeats = 0;
            StringBuilder builder = null;
            for (int i = 0; i < self.Length; i++)
            {
                var c = self[i];
                if (c.IsLineSeparator())
                {
                    repeats++;

                    if (builder == null)
                    {
                        builder = new StringBuilder(self.Length)
                            .Append(self, 0, i);
                    }

                    if (!collapse || repeats == 1)
                    {
                        builder.Append(' ');
                    }
                }
                else if (builder != null)
                {
                    repeats = 0;

                    builder.Append(c);
                }
            }

            return builder == null ? self : builder.ToString();
        }

#if !SILVERLIGHT
        /// <summary>
        /// Converts the specified string to titlecase.
        /// </summary>
        /// <returns> The specified string converted to titlecase.</returns>
        /// <param name="self">The string to convert to titlecase.</param>
        /// <exception cref="T:System.ArgumentNullException">When <paramref name="self"/> is null.</exception>
        public static string ToTitleCase(this string self)
        {
            CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
            TextInfo textInfo = cultureInfo.TextInfo;
            return textInfo.ToTitleCase(self);
        }

        /// <summary>
        /// Converts the specified string to titlecase.
        /// </summary>
        /// <returns>The specified string converted to titlecase.</returns>
        /// <param name="self">The string to convert to titlecase.</param>
        /// <param name="culture"></param>
        /// <exception cref="T:System.ArgumentNullException">self is null.</exception>
        public static string ToTitleCase(this string self, CultureInfo culture)
        {
            TextInfo textInfo = culture.TextInfo;
            return textInfo.ToTitleCase(self);
        }
#endif

        [NotNull]
        [JetBrains.Annotations.Pure]
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
