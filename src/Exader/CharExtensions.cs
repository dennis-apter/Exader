using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Exader
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public static class CharExtensions
    {
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Equals(this char self, char other, StringComparison comparison)
        {
            return self == other || self.ToString().Equals(other.ToString(), comparison);
        }
        
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool EqualsIgnoreCase(this char self, char other)
        {
            return self == other || self.ToString().Equals(other.ToString(), StringExtensions.IgnoreCase);
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsLower(this char c)
        {
            return c == c.ToLower();
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsQuotationMark(this char self)
        {
            return IsOpenOrCloseQuotationMark(self) || 
                IsOpenQuotationMark(self) || 
                IsCloseQuotationMark(self);
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsQuotationMarks(this char first, char last)
        {
            return (first.IsOpenQuotationMark() && last.IsCloseQuotationMark()) ||
                (first.IsOpenOrCloseQuotationMark() && last.IsOpenOrCloseQuotationMark());
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsUpper(this char c)
        {
            return c == c.ToUpper();
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static char ToLower(this char c)
        {
            return char.ToLower(c);
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static char ToLowerInvariant(this char c)
        {
            return char.ToLowerInvariant(c);
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static char ToUpper(this char c)
        {
            return char.ToUpper(c);
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsLineSeparator(this char self)
        {
            switch (self)
            {
                case '\r':
                case '\n':
                    return true;
                default:
                    var category = CharUnicodeInfo.GetUnicodeCategory(self);
                    return category == UnicodeCategory.LineSeparator
                        || category == UnicodeCategory.ParagraphSeparator;
            }
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static char ToUpperInvariant(this char c)
        {
            return char.ToUpperInvariant(c);
        }
    }
}
