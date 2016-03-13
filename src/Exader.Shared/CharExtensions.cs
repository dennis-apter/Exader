using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Exader
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public static class CharExtensions
    {
#if SILVERLIGHT || NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [Pure]
        public static bool Equals(this char self, char other, StringComparison comparison)
        {
            return self == other || self.ToString().Equals(other.ToString(), comparison);
        }

#if SILVERLIGHT || NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [Pure]
        public static bool EqualsIgnoreCase(this char self, char other)
        {
            return self == other || self.ToString().Equals(other.ToString(), StringComparison.InvariantCultureIgnoreCase);
        }

#if SILVERLIGHT || NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [Pure]
        public static bool IsCloseQuotationMark(this char self)
        {
            switch (self)
            {
                case '\u00BB': // » ru, fr
                case '\u203A': // › nested: fr
                case '\u201D': // ” en
                case '\u2019': // ’ nested: en
                    return true;
                default:
                    return false;
            }
        }

#if SILVERLIGHT || NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [Pure]
        public static bool IsLineSeparator(this char self)
        {
            switch (self)
            {
                case '\r':
                case '\n':
                    return true;
                default:
                    var category = char.GetUnicodeCategory(self);
                    return category == UnicodeCategory.LineSeparator
                        || category == UnicodeCategory.ParagraphSeparator;
            }
        }

#if SILVERLIGHT || NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [Pure]
        public static bool IsLower(this char c)
        {
            return c == c.ToLower();
        }

#if SILVERLIGHT || NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [Pure]
        public static bool IsOpenOrCloseQuotationMark(this char self)
        {
            switch (self)
            {
                case '\'':
                case '"':
                case '\u201C': // “ open: en; close: ru, de
                case '\u2018': // ‘ open: en; close: ru
                    return true;
                default:
                    return false;
            }
        }

#if SILVERLIGHT || NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [Pure]
        public static bool IsOpenQuotationMark(this char self)
        {
            switch (self)
            {
                case '\u00AB': // « ru, fr
                case '\u2039': // ‹ nested: fr
                case '\u201E': // „ nested: ru, de
                case '\u201A': // ‚ nested: eu
                    // rare
                case '\u201F': // ‟ en
                case '\u201B': // ‛ nested: en
                    return true;
                default:
                    return false;
            }
        }

#if SILVERLIGHT || NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [Pure]
        public static bool IsQuotationMark(this char self)
        {
            return IsOpenOrCloseQuotationMark(self) || 
                IsOpenQuotationMark(self) || 
                IsCloseQuotationMark(self);
        }

#if SILVERLIGHT || NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [Pure]
        public static bool IsQuotationMarks(this char first, char last)
        {
            return (first.IsOpenQuotationMark() && last.IsCloseQuotationMark()) ||
                (first.IsOpenOrCloseQuotationMark() && last.IsOpenOrCloseQuotationMark());
        }

#if SILVERLIGHT || NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [Pure]
        public static bool IsUpper(this char c)
        {
            return c == c.ToUpper();
        }

#if SILVERLIGHT || NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [Pure]
        public static char ToLower(this char c)
        {
            return char.ToLower(c);
        }

#if SILVERLIGHT || NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [Pure]
        public static char ToLower(this char c, CultureInfo culture)
        {
            return char.ToLower(c, culture);
        }

#if SILVERLIGHT || NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [Pure]
        public static char ToLowerInvariant(this char c)
        {
            return char.ToLowerInvariant(c);
        }

#if SILVERLIGHT || NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [Pure]
        public static char ToUpper(this char c)
        {
            return char.ToUpper(c);
        }

#if SILVERLIGHT || NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [Pure]
        public static char ToUpper(this char c, CultureInfo culture)
        {
            return char.ToUpper(c, culture);
        }

#if SILVERLIGHT || NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [Pure]
        public static char ToUpperInvariant(this char c)
        {
            return char.ToUpperInvariant(c);
        }
    }
}
