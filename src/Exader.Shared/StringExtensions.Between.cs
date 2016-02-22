using System;
using System.Globalization;
using JetBrains.Annotations;

namespace Exader
{
    public static partial class StringExtensions
    {
        [NotNull]
        public static string Between(
            this string self,
            string token,
            bool include = false,
            CultureInfo culture = null,
            CompareOptions compareOptions = CompareOptions.Ordinal)
        {
            return BetweenInternal(false, self, token, token, include, culture, compareOptions);
        }

        [NotNull]
        public static string Between(
            this string self,
            string startToken,
            string endToken,
            bool include = false,
            CultureInfo culture = null,
            CompareOptions compareOptions = CompareOptions.Ordinal)
        {
            return BetweenInternal(false, self, startToken, endToken, include, culture, compareOptions);
        }

        [NotNull]
        public static string Between(
            this string self,
            string startToken,
            string endToken,
            out string left,
            out string right,
            CultureInfo culture = null,
            CompareOptions compareOptions = CompareOptions.Ordinal)
        {
            return BetweenInternal(false, self, startToken, endToken, out left, out right, culture, compareOptions);
        }

        [NotNull]
        public static string Between(
            this string self,
            char startToken,
            char endToken,
            out string left,
            out string right,
            CultureInfo culture = null,
            CompareOptions compareOptions = CompareOptions.Ordinal)
        {
            return BetweenInternal(false, self, startToken, endToken, out left, out right, culture, compareOptions);
        }

        [NotNull]
        public static string Between(
            this string self,
            char token,
            bool include = false,
            CultureInfo culture = null,
            CompareOptions compareOptions = CompareOptions.Ordinal)
        {
            return BetweenInternal(false, self, token, token, include, culture, compareOptions);
        }

        [NotNull]
        public static string Between(
            this string self,
            char startToken,
            char endToken,
            bool include = false,
            CultureInfo culture = null,
            CompareOptions compareOptions = CompareOptions.Ordinal)
        {
            return BetweenInternal(false, self, startToken, endToken, include, culture, compareOptions);
        }

        [NotNull]
        public static string BetweenLast(
            this string self,
            string token,
            bool include = false,
            CultureInfo culture = null,
            CompareOptions compareOptions = CompareOptions.Ordinal)
        {
            return BetweenInternal(true, self, token, token, include, culture, compareOptions);
        }

        [NotNull]
        public static string BetweenLast(
            this string self,
            char token,
            bool include = false,
            CultureInfo culture = null,
            CompareOptions compareOptions = CompareOptions.Ordinal)
        {
            return BetweenInternal(true, self, token, token, include, culture, compareOptions);
        }

        [NotNull]
        public static string BetweenLast(
            this string self,
            string startToken,
            string endToken,
            bool include = false,
            CultureInfo culture = null,
            CompareOptions compareOptions = CompareOptions.Ordinal)
        {
            return BetweenInternal(true, self, startToken, endToken, include, culture, compareOptions);
        }

        [NotNull]
        public static string BetweenLast(
            this string self,
            string startToken,
            string endToken,
            out string left,
            out string right,
            CultureInfo culture = null,
            CompareOptions compareOptions = CompareOptions.Ordinal)
        {
            return BetweenInternal(true, self, startToken, endToken, out left, out right, culture, compareOptions);
        }

        [NotNull]
        internal static string BetweenLast(
            this string self,
            char startToken,
            char endToken,
            out string left,
            out string right,
            CultureInfo culture = null,
            CompareOptions compareOptions = CompareOptions.Ordinal)
        {
            return BetweenInternal(true, self, startToken, endToken, out left, out right, culture, compareOptions);
        }

        [NotNull]
        internal static string BetweenLast(
            this string self,
            char startToken,
            char endToken,
            bool include = false,
            CultureInfo culture = null,
            CompareOptions compareOptions = CompareOptions.Ordinal)
        {
            return BetweenInternal(true, self, startToken, endToken, include, culture, compareOptions);
        }

        [Obsolete("TODO")]
        [NotNull]
        internal static string BetweenLastOrSelf(
            this string self,
            string startToken,
            string endToken,
            bool include = false,
            CultureInfo culture = null,
            CompareOptions compareOptions = CompareOptions.Ordinal)
        {
            var result = BetweenInternal(true, self, startToken, endToken, include, culture, compareOptions);
            return result == string.Empty ? self : result;
        }

        [Obsolete("TODO")]
        [NotNull]
        internal static string BetweenLastOrSelf(
            this string self,
            char startToken,
            char endToken,
            bool include = false,
            CultureInfo culture = null,
            CompareOptions compareOptions = CompareOptions.Ordinal)
        {
            var result = BetweenInternal(true, self, startToken, endToken, include, culture, compareOptions);
            return result == string.Empty ? self : result;
        }

        [NotNull]
        public static string BetweenOrSelf(
            this string self,
            string startToken,
            string endToken,
            bool include = false,
            CultureInfo culture = null,
            CompareOptions compareOptions = CompareOptions.Ordinal)
        {
            var result = BetweenInternal(false, self, startToken, endToken, false, culture, compareOptions);
            if (result != string.Empty)
            {
                return include
                    ? startToken + result + endToken
                    : result;
            }

            return self;
        }

        [NotNull]
        public static string BetweenOrSelf(
            this string self,
            char startToken,
            char endToken,
            bool include = false,
            CultureInfo culture = null,
            CompareOptions compareOptions = CompareOptions.Ordinal)
        {
            var result = BetweenInternal(false, self, startToken, endToken, false, culture, compareOptions);
            if (result != string.Empty)
            {
                return include
                    ? startToken + result + endToken
                    : result;
            }

            return self;
        }

        [NotNull]
        private static string BetweenInternal(
            bool last,
            string self,
            string startToken,
            string endToken,
            bool include,
            CultureInfo culture,
            CompareOptions compareOptions)
        {
            if (string.IsNullOrEmpty(self) ||
                string.IsNullOrEmpty(startToken) ||
                string.IsNullOrEmpty(endToken))
            {
                return string.Empty;
            }

            if (null == culture)
                culture = CultureInfo.InvariantCulture;

            if (last)
            {
                int ei = culture.CompareInfo.LastIndexOf(self, endToken, compareOptions);
                if (ei < 0)
                    return string.Empty;

                int si = culture.CompareInfo.LastIndexOf(self, startToken, ei, compareOptions);
                if (si <= ei)
                {
                    if (include)
                    {
                        ei += endToken.Length;
                    }
                    else
                    {
                        si += startToken.Length;
                    }

                    return self.Substring(si, ei - si);
                }
            }
            else
            {
                int si = culture.CompareInfo.IndexOf(self, startToken, compareOptions);
                if (si < 0)
                    return string.Empty;

                var offset = si + startToken.Length;
                if (!include)
                    si = offset;

                int ei = culture.CompareInfo.IndexOf(self, endToken, offset, compareOptions);
                if (si <= ei)
                {
                    if (include)
                        ei += endToken.Length;

                    return self.Substring(si, ei - si);
                }
            }

            return string.Empty;
        }

        [NotNull]
        private static string BetweenInternal(
            bool last,
            string self,
            string startToken,
            string endToken,
            out string left,
            out string right,
            CultureInfo culture,
            CompareOptions compareOptions)
        {
            left = string.Empty;
            right = string.Empty;
            if (string.IsNullOrEmpty(self) ||
                string.IsNullOrEmpty(startToken) ||
                string.IsNullOrEmpty(endToken))
            {
                return string.Empty;
            }

            if (null == culture)
                culture = CultureInfo.InvariantCulture;

            if (last)
            {
                int ei = culture.CompareInfo.LastIndexOf(self, endToken, compareOptions);
                if (ei < 0)
                    return string.Empty;

                right = self.SubstringAfter(ei);

                int si = culture.CompareInfo.LastIndexOf(self, startToken, ei, compareOptions);
                if (si <= ei)
                {
                    si += startToken.Length;
                    left = self.Left(si);
                    return self.Substring(si, ei - si);
                }

                left = string.Empty;
            }
            else
            {
                int si = culture.CompareInfo.IndexOf(self, startToken, compareOptions);
                if (si < 0)
                    return string.Empty;

                left = self.Left(si);
                si += startToken.Length;

                int ei = culture.CompareInfo.IndexOf(self, endToken, si, compareOptions);
                if (si <= ei)
                {
                    right = self.SubstringAfter(ei + endToken.Length);
                    return self.Substring(si, ei - si);
                }

                left = string.Empty;
            }

            return string.Empty;
        }

        [NotNull]
        private static string BetweenInternal(
            bool last,
            string self,
            char startToken,
            char endToken,
            out string left,
            out string right,
            CultureInfo culture,
            CompareOptions compareOptions)
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
                    return string.Empty;

                right = self.SubstringAfter(ei);

                int si = culture.CompareInfo.LastIndexOf(self, startToken, ei - 1, compareOptions);
                if (si <= ei)
                {
                    si++;
                    left = self.Left(si);
                    return self.Substring(si, ei - si);
                }

                left = string.Empty;
            }
            else
            {
                int si = culture.CompareInfo.IndexOf(self, startToken, compareOptions);
                if (si < 0)
                    return string.Empty;

                left = self.Left(si);
                si++;

                int ei = compareInfo.IndexOf(self, endToken, si, compareOptions);
                if (si <= ei)
                {
                    right = self.SubstringAfter(ei + 1);
                    return self.Substring(si, ei - si);
                }

                left = string.Empty;
            }

            return string.Empty;
        }

        [NotNull]
        private static string BetweenInternal(
            bool last,
            string self,
            char startToken,
            char endToken,
            bool include,
            CultureInfo culture,
            CompareOptions compareOptions)
        {
            if (null == culture)
                culture = CultureInfo.InvariantCulture;

            var compareInfo = culture.CompareInfo;
            if (last)
            {
                int ei = culture.CompareInfo.LastIndexOf(self, endToken, compareOptions);
                if (ei < 0)
                    return string.Empty;

                int si = culture.CompareInfo.LastIndexOf(self, startToken, ei - 1, compareOptions);
                if (si <= ei)
                {
                    if (include)
                        ei += 1;
                    else
                        si += 1;

                    return self.Substring(si, ei - si);
                }
            }
            else
            {
                int si = culture.CompareInfo.IndexOf(self, startToken, compareOptions);
                if (si < 0)
                    return string.Empty;

                var offset = si + 1;
                if (!include)
                    si = offset;

                int ei = compareInfo.IndexOf(self, endToken, offset, compareOptions);
                if (si <= ei)
                {
                    if (include)
                        ei += 1;

                    return self.Substring(si, ei - si);
                }
            }

            return string.Empty;
        }
    }
}
