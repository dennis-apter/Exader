using System;
using System.IO;
using System.Text;
using JetBrains.Annotations;
#if !SILVERLIGHT
using System.Security;
#endif

namespace Exader
{
    public static partial class StringExtensions
    {
        #region Methods

        public static bool IsBoolean(this string value)
        {
            bool temp;
            return bool.TryParse(value, out temp);
        }

        public static bool IsByte(this string value)
        {
            byte temp;
            return byte.TryParse(value, out temp);
        }

        public static bool IsDateTime(this string value)
        {
            DateTime temp;
            return DateTime.TryParse(value, out temp);
        }

        public static bool IsDecimal(this string value)
        {
            decimal temp;
            return decimal.TryParse(value, out temp);
        }

        public static bool IsInt16(this string value)
        {
            short temp;
            return short.TryParse(value, out temp);
        }

        public static bool IsInt32(this string value)
        {
            int temp;
            return int.TryParse(value, out temp);
        }

        public static bool IsInt64(this string value)
        {
            long temp;
            return long.TryParse(value, out temp);
        }

        public static StringBuilder ToBuilder(this string self)
        {
            return new StringBuilder(self);
        }

        public static StringReader ToReader([NotNull] this string self)
        {
            return new StringReader(self);
        }

#if !SILVERLIGHT
        public static SecureString ToSecureString(this string self)
        {
            if (null == self)
            {
                throw new ArgumentNullException("self");
            }

            var ss = new SecureString();
            foreach (char c in self)
            {
                ss.AppendChar(c);
            }

            return ss;
        }
#endif

        public static Stream ToStream([NotNull] this string self, Encoding encoding = null)
        {
#if SILVERLIGHT
            if (null == encoding) encoding = Encoding.UTF8;
#else
            if (null == encoding) encoding = Encoding.Default;
#endif
            return new MemoryStream(encoding.GetBytes(self));
        }

        #endregion
    }
}