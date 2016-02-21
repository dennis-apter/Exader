using System;

namespace Exader
{
    public static class GuidExtensions
    {
        /// <summary>
        /// Конвертирует заданную <see cref="string"/> в формате Base64,
        /// содержащую закодированнный методом <see cref="ToBase64String"/> в <see cref="Guid"/>.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Guid FromBase64String(string value)
        {
            return new Guid(Convert.FromBase64String(value));
        }

        public static string ToBase64String(this Guid value)
        {
            return Convert.ToBase64String(value.ToByteArray());
        }
    }
}
