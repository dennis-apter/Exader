using System;
using System.Globalization;
using JetBrains.Annotations;

namespace Exader
{
    public static partial class StringExtensions
    {
        [NotNull]
        [Pure]
        public static string Between(
            this string self,
            string token,
            bool include = false,
            CultureInfo culture = null,
            CompareOptions compareOptions = CompareOptions.Ordinal)
        {
            return BetweenCore(false, self, token, token, include, culture, compareOptions, "");
        }

        [NotNull]
        [Pure]
        public static string Between(
            this string self,
            string startToken,
            string endToken,
            bool include = false,
            CultureInfo culture = null,
            CompareOptions compareOptions = CompareOptions.Ordinal)
        {
            return BetweenCore(false, self, startToken, endToken, include, culture, compareOptions, "");
        }

        [NotNull]
        [Pure]
        public static string Between(
            this string self,
            string startToken,
            string endToken,
            out string left,
            out string right,
            CultureInfo culture = null,
            CompareOptions compareOptions = CompareOptions.Ordinal)
        {
            return BetweenCore(false, self, startToken, endToken, out left, out right, culture, compareOptions, "");
        }

        [NotNull]
        [Pure]
        public static string Between(
            this string self,
            char startToken,
            char endToken,
            out string left,
            out string right,
            CultureInfo culture = null,
            CompareOptions compareOptions = CompareOptions.Ordinal)
        {
            return BetweenCore(false, self, startToken, endToken, out left, out right, culture, compareOptions, "");
        }

        [NotNull]
        [Pure]
        public static string Between(
            this string self,
            char token,
            bool include = false,
            CultureInfo culture = null,
            CompareOptions compareOptions = CompareOptions.Ordinal)
        {
            return BetweenCore(false, self, token, token, include, culture, compareOptions, "");
        }

        [NotNull]
        [Pure]
        public static string Between(
            this string self,
            char startToken,
            char endToken,
            bool include = false,
            CultureInfo culture = null,
            CompareOptions compareOptions = CompareOptions.Ordinal)
        {
            return BetweenCore(false, self, startToken, endToken, include, culture, compareOptions, "");
        }

        [NotNull]
        [Pure]
        public static string BetweenLast(
            this string self,
            string token,
            bool include = false,
            CultureInfo culture = null,
            CompareOptions compareOptions = CompareOptions.Ordinal)
        {
            return BetweenCore(true, self, token, token, include, culture, compareOptions, "");
        }

        [NotNull]
        [Pure]
        public static string BetweenLast(
            this string self,
            char token,
            bool include = false,
            CultureInfo culture = null,
            CompareOptions compareOptions = CompareOptions.Ordinal)
        {
            return BetweenCore(true, self, token, token, include, culture, compareOptions,"");
        }

        [NotNull]
        [Pure]
        public static string BetweenLast(
            this string self,
            string startToken,
            string endToken,
            bool include = false,
            CultureInfo culture = null,
            CompareOptions compareOptions = CompareOptions.Ordinal)
        {
            return BetweenCore(true, self, startToken, endToken, include, culture, compareOptions, "");
        }

        [NotNull]
        [Pure]
        public static string BetweenLast(
            this string self,
            string startToken,
            string endToken,
            out string left,
            out string right,
            CultureInfo culture = null,
            CompareOptions compareOptions = CompareOptions.Ordinal)
        {
            return BetweenCore(true, self, startToken, endToken, out left, out right, culture, compareOptions, "");
        }

        [NotNull]
        [Pure]
        public static string BetweenLast(
            this string self,
            char startToken,
            char endToken,
            out string left,
            out string right,
            CultureInfo culture = null,
            CompareOptions compareOptions = CompareOptions.Ordinal)
        {
            return BetweenCore(true, self, startToken, endToken, out left, out right, culture, compareOptions, "");
        }

        [NotNull]
        [Pure]
        public static string BetweenLast(
            this string self,
            char startToken,
            char endToken,
            bool include = false,
            CultureInfo culture = null,
            CompareOptions compareOptions = CompareOptions.Ordinal)
        {
            return BetweenCore(true, self, startToken, endToken, include, culture, compareOptions, "");
        }
        
        [NotNull]
        [Pure]
        public static string BetweenLastOrSelf(
            this string self,
            string startToken,
            string endToken,
            bool include = false,
            CultureInfo culture = null,
            CompareOptions compareOptions = CompareOptions.Ordinal)
        {
            var result = BetweenCore(true, self, startToken, endToken, include, culture, compareOptions, self);
            return result;
        }
        
        [NotNull]
        [Pure]
        public static string BetweenLastOrSelf(
            this string self,
            char startToken,
            char endToken,
            bool include = false,
            CultureInfo culture = null,
            CompareOptions compareOptions = CompareOptions.Ordinal)
        {
            var result = BetweenCore(true, self, startToken, endToken, include, culture, compareOptions, self);
            return result;
        }

        [NotNull]
        [Pure]
        public static string BetweenOrSelf(
            this string self,
            string startToken,
            string endToken,
            bool include = false,
            CultureInfo culture = null,
            CompareOptions compareOptions = CompareOptions.Ordinal)
        {
            var result = BetweenCore(false, self, startToken, endToken, false, culture, compareOptions, self);
            return result;
        }

        [NotNull]
        [Pure]
        public static string BetweenOrSelf(
            this string self,
            char startToken,
            char endToken,
            bool include = false,
            CultureInfo culture = null,
            CompareOptions compareOptions = CompareOptions.Ordinal)
        {
            var result = BetweenCore(false, self, startToken, endToken, false, culture, compareOptions, self);
            return result;
        }

        [NotNull]
        private static string BetweenCore(
            bool last,
            string self,
            string startToken,
            string endToken,
            bool include,
            CultureInfo culture,
            CompareOptions compareOptions,
            string defaultValue)
        {
            if (string.IsNullOrEmpty(self) ||
                string.IsNullOrEmpty(startToken) ||
                string.IsNullOrEmpty(endToken))
            {
                return defaultValue;
            }

            if (null == culture)
                culture = CultureInfo.InvariantCulture;

            if (last)
            {
                int ei = culture.CompareInfo.LastIndexOf(self, endToken, compareOptions);
                if (ei < 0)
                    return defaultValue;

                int si = culture.CompareInfo.LastIndexOf(self, startToken, ei - 1, compareOptions);
                if (si <= ei)
                {
                    if (include)
                    {
                        if (ei - si == startToken.Length)
                            return defaultValue;

                        ei += endToken.Length;
                    }
                    else
                    {
                        si += startToken.Length;
                    }

                    if (si == ei)
                        return defaultValue;

                    return self.Substring(si, ei - si);
                }
            }
            else
            {
                int si = culture.CompareInfo.IndexOf(self, startToken, compareOptions);
                if (si < 0)
                    return defaultValue;

                var offset = si + startToken.Length;
                if (!include)
                    si = offset;

                int ei = culture.CompareInfo.IndexOf(self, endToken, offset, compareOptions);
                if (si <= ei)
                {
                    if (include)
                        ei += endToken.Length;

                    if (si == ei)
                        return defaultValue;

                    return self.Substring(si, ei - si);
                }
            }

            return defaultValue;
        }

        [NotNull]
        private static string BetweenCore(
            bool last,
            string self,
            string startToken,
            string endToken,
            out string left,
            out string right,
            CultureInfo culture,
            CompareOptions compareOptions,
            string defaultValue)
        {
            left = string.Empty;
            right = string.Empty;
            if (string.IsNullOrEmpty(self) ||
                string.IsNullOrEmpty(startToken) ||
                string.IsNullOrEmpty(endToken))
            {
                return defaultValue;
            }

            if (null == culture)
                culture = CultureInfo.InvariantCulture;

            if (last)
            {
                int ei = culture.CompareInfo.LastIndexOf(self, endToken, compareOptions);
                if (ei < 0)
                    return defaultValue;

                right = self.SubstringAfter(ei);

                int si = culture.CompareInfo.LastIndexOf(self, startToken, ei - 1, compareOptions);
                if (si <= ei)
                {
                    si += startToken.Length;
                    left = self.Left(si);

                    if (si == ei)
                        return defaultValue;

                    return self.Substring(si, ei - si);
                }

                left = string.Empty;
            }
            else
            {
                int si = culture.CompareInfo.IndexOf(self, startToken, compareOptions);
                if (si < 0)
                    return defaultValue;

                si += startToken.Length;
                left = self.Left(si);

                int ei = culture.CompareInfo.IndexOf(self, endToken, si, compareOptions);
                if (si <= ei)
                {
                    right = self.SubstringAfter(ei);

                    if (si == ei)
                        return defaultValue;

                    return self.Substring(si, ei - si);
                }

                left = string.Empty;
            }

            return defaultValue;
        }

        [NotNull]
        private static string BetweenCore(
            bool last,
            string self,
            char startToken,
            char endToken,
            out string left,
            out string right,
            CultureInfo culture,
            CompareOptions compareOptions,
            string defaultValue)
        {
            left = string.Empty;
            right = string.Empty;
            if (null == culture)
                culture = CultureInfo.InvariantCulture;

            var compareInfo = culture.CompareInfo;
            if (last)
            {
                int ei = culture.CompareInfo.LastIndexOf(self, endToken, compareOptions);
                if (ei < 0)
                    return defaultValue;

                right = self.SubstringAfter(ei);

                int si = culture.CompareInfo.LastIndexOf(self, startToken, ei - 1, compareOptions);
                if (si <= ei)
                {
                    si++;
                    left = self.Left(si);

                    if (si == ei)
                        return defaultValue;

                    return self.Substring(si, ei - si);
                }

                left = string.Empty;
            }
            else
            {
                int si = culture.CompareInfo.IndexOf(self, startToken, compareOptions);
                if (si < 0)
                    return defaultValue;

                si++;
                left = self.Left(si);

                int ei = compareInfo.IndexOf(self, endToken, si, compareOptions);
                if (si <= ei)
                {
                    right = self.SubstringAfter(ei);

                    if (si == ei)
                        return defaultValue;

                    return self.Substring(si, ei - si);
                }

                left = string.Empty;
            }

            return defaultValue;
        }

        [NotNull]
        private static string BetweenCore(
            bool last,
            string self,
            char startToken,
            char endToken,
            bool include,
            CultureInfo culture,
            CompareOptions compareOptions,
            string defaultValue)
        {
            if (null == culture)
                culture = CultureInfo.InvariantCulture;

            var compareInfo = culture.CompareInfo;
            if (last)
            {
                int ei = culture.CompareInfo.LastIndexOf(self, endToken, compareOptions);
                if (ei < 0)
                    return defaultValue;

                int si = culture.CompareInfo.LastIndexOf(self, startToken, ei - 1, compareOptions);
                if (si <= ei)
                {
                    if (include)
                    {
                        if (ei - si == 1)
                            return defaultValue;

                        ei += 1;
                    }
                    else
                    {
                        si += 1;
                    }

                    if (si == ei)
                        return defaultValue;

                    return self.Substring(si, ei - si);
                }
            }
            else
            {
                int si = culture.CompareInfo.IndexOf(self, startToken, compareOptions);
                if (si < 0)
                    return defaultValue;

                var offset = si + 1;
                if (!include)
                    si = offset;

                int ei = compareInfo.IndexOf(self, endToken, offset, compareOptions);
                if (si <= ei)
                {
                    if (include)
                        ei += 1;

                    if (si == ei)
                        return defaultValue;

                    return self.Substring(si, ei - si);
                }
            }

            return defaultValue;
        }
    }
}
