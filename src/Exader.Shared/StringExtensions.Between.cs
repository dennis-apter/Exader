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
            {
                culture = CultureInfo.InvariantCulture;
            }

            int startIndex = last
                ? culture.CompareInfo.LastIndexOf(self, startToken, compareOptions)
                : culture.CompareInfo.IndexOf(self, startToken, compareOptions);

            if (startIndex < 0)
            {
                return string.Empty;
            }

            var endOffset = startIndex + startToken.Length;
            if (!include)
            {
                startIndex = endOffset;
            }

            int endIndex = last
                ? culture.CompareInfo.LastIndexOf(self, endToken, endOffset, compareOptions)
                : culture.CompareInfo.IndexOf(self, endToken, endOffset, compareOptions);

            if (startIndex <= endIndex)
            {
                if (include)
                {
                    endIndex += endToken.Length;
                }

                return self.Substring(startIndex, endIndex - startIndex);
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
            {
                culture = CultureInfo.InvariantCulture;
            }

            int startIndex = last
                ? culture.CompareInfo.LastIndexOf(self, startToken, compareOptions)
                : culture.CompareInfo.IndexOf(self, startToken, compareOptions);

            if (startIndex < 0)
            {
                return string.Empty;
            }
            else
            {
                left = self.Left(startIndex);
                startIndex += startToken.Length;
            }

            int endIndex = culture.CompareInfo.IndexOf(self, endToken, startIndex, compareOptions);
            if (startIndex <= endIndex)
            {
                right = self.SubstringAfter(endIndex + endToken.Length);
                return self.Substring(startIndex, endIndex - startIndex);
            }
            else
            {
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
            {
                culture = CultureInfo.InvariantCulture;
            }

            var compareInfo = culture.CompareInfo;
            int startIndex = last
                ? culture.CompareInfo.LastIndexOf(self, startToken, compareOptions)
                : culture.CompareInfo.IndexOf(self, startToken, compareOptions);

            if (startIndex < 0)
            {
                return string.Empty;
            }
            else
            {
                left = self.Left(startIndex);
                startIndex++;
            }

            int endIndex = compareInfo.IndexOf(self, endToken, startIndex, compareOptions);
            if (startIndex <= endIndex)
            {
                right = self.SubstringAfter(endIndex + 1);
                return self.Substring(startIndex, endIndex - startIndex);
            }
            else
            {
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
            {
                culture = CultureInfo.InvariantCulture;
            }

            var compareInfo = culture.CompareInfo;
            int startIndex = last
                ? culture.CompareInfo.LastIndexOf(self, startToken, compareOptions)
                : culture.CompareInfo.IndexOf(self, startToken, compareOptions);

            if (startIndex < 0)
            {
                return string.Empty;
            }

            var endOffset = startIndex + 1;
            if (!include)
            {
                startIndex = endOffset;
            }

            int endIndex = compareInfo.IndexOf(self, endToken, endOffset, compareOptions);
            if (startIndex <= endIndex)
            {
                if (include)
                {
                    endIndex += 1;
                }

                return self.Substring(startIndex, endIndex - startIndex);
            }
            
            return string.Empty;
        }
        
        [Obsolete("TODO")]
        [NotNull]
        internal static string BetweenLast(
            this string self,
            string token,
            bool include = false,
            CultureInfo culture = null,
            CompareOptions compareOptions = CompareOptions.Ordinal)
        {
            return BetweenInternal(true, self, token, token, include, culture, compareOptions);
        }
        
        [Obsolete("TODO")]
        [NotNull]
        internal static string BetweenLast(
            this string self,
            char token,
            bool include = false,
            CultureInfo culture = null,
            CompareOptions compareOptions = CompareOptions.Ordinal)
        {
            return BetweenInternal(true, self, token, token, include, culture, compareOptions);
        }
        
        [Obsolete("TODO")]
        [NotNull]
        internal static string BetweenLast(
            this string self,
            string startToken,
            string endToken,
            bool include = false,
            CultureInfo culture = null,
            CompareOptions compareOptions = CompareOptions.Ordinal)
        {
            return BetweenInternal(true, self, startToken, endToken, include, culture, compareOptions);
        }
        
        [Obsolete("TODO")]
        [NotNull]
        internal static string BetweenLast(
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

        [Obsolete("TODO")]
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
    }
}
