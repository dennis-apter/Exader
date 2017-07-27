using System;
using System.IO;
using System.Text;
using JetBrains.Annotations;

namespace Exader
{
    public static partial class StringExtensions
    {
        [Pure]
        public static bool IsBoolean(this string value)
        {
            return bool.TryParse(value, out _);
        }
        
        [Pure]
        public static bool IsByte(this string value)
        {
            return byte.TryParse(value, out _);
        }
        
        [Pure]
        public static bool IsDateTime(this string value)
        {
            return DateTime.TryParse(value, out _);
        }
        
        [Pure]
        public static bool IsDecimal(this string value)
        {
            return decimal.TryParse(value, out _);
        }
        
        [Pure]
        public static bool IsInt16(this string value)
        {
            return short.TryParse(value, out _);
        }
        
        [Pure]
        public static bool IsInt32(this string value)
        {
            return int.TryParse(value, out _);
        }
        
        [Pure]
        public static bool IsInt64(this string value)
        {
            return long.TryParse(value, out _);
        }
        
        [Pure]
        public static StringBuilder ToBuilder(this string self)
        {
            return new StringBuilder(self);
        }
        
        [Pure]
        public static StringReader ToReader([NotNull] this string self)
        {
            return new StringReader(self);
        }

        [Pure]
        public static Stream ToStream([NotNull] this string self, Encoding encoding = null)
        {
            if (null == encoding) encoding = Encoding.UTF8;

            return new MemoryStream(encoding.GetBytes(self));
        }
    }
}
