using System;
using System.ComponentModel;
using System.Globalization;
using Exader.Reflection;

namespace Exader
{
    public static class EnumExtensions
    {
        public static string GetDescription(this Enum item, string defaultDescription = null)
        {
            Type type = item.GetType();
            string name = Enum.GetName(type, item);
            if (!string.IsNullOrEmpty(name))
            {
                var attribute = type.GetField(name).GetCustomAttribute<DescriptionAttribute>();
                return null != attribute ? attribute.Description : (defaultDescription ?? name);
            }

            return null;
        }

        public static object GetValue(this Enum item)
        {
            Type underlyingType = Enum.GetUnderlyingType(item.GetType());
            return Convert.ChangeType(item, underlyingType, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Возращает значение обозначающее наличие флаг в указанномо наборе.
        /// </summary>
        /// <remarks>
        /// Вызов этого метода обходится в 250–300 раз дороже бинарного аналога
        /// (при повторе 1'000'000 раз — 1250–1400 и 5–7 милисекунд соответсвенно).
        /// Хотя эта разница и может показаться впечатляющей, чаще выгодней написать
        /// более понятный код, чем гнаться за несущественным ускорением.
        /// </remarks>
        /// <param name="flags"></param>
        /// <param name="self"></param>
        /// <returns></returns>
        public static bool HasFlags<T>(this T self, T flags) where T : struct
        {
            long lfs = Convert.ToInt64(self);
            long lf = Convert.ToInt64(flags);
            return lf == (lfs & lf);
        }

        [Obsolete]
        public static bool In(this Enum flag, Enum flags)
        {
            long lfs = Convert.ToInt64(flags);
            long lf = Convert.ToInt64(flag);
            return lf == (lfs & lf);
        }

        public static T Parse<T>(string value) where T : struct
        {
            return (T)Enum.Parse(typeof(T), value, false);
        }

        public static T Parse<T>(string value, bool ignoreCase) where T : struct
        {
            return (T)Enum.Parse(typeof(T), value, ignoreCase);
        }

        public static T ParseOrSelf<T>(this T self, string value) where T : struct
        {
            return self.ParseOrSelf(value, false);
        }

        public static T ParseOrSelf<T>(this T self, string value, bool ignoreCase) where T : struct
        {
            if (!string.IsNullOrEmpty(value))
            {
                try
                {
                    return (T)Enum.Parse(typeof(T), value, ignoreCase);
                }
                catch (ArgumentException)
                {
                }
            }

            return self;
        }

        public static bool TryParse<T>(string value, out T result) where T : struct
        {
            return TryParse(value, false, out result);
        }

        public static bool TryParse<T>(string value, bool ignoreCase, out T result) where T : struct
        {
            try
            {
                result = Parse<T>(value, ignoreCase);
                return true;
            }
            catch (FormatException)
            {
                result = default(T);
                return false;
            }
        }
    }
}