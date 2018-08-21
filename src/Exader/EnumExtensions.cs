#if NET45
using System;
using System.ComponentModel;
using System.Reflection;

namespace Exader
{
    public static class EnumExtensions
    {
        public static string GetDescription(this Enum item, string defaultDescription = null)
        {
            var type = item.GetType();
            string name = Enum.GetName(type, item);
            if (!string.IsNullOrEmpty(name))
            {
                var attribute = type.GetField(name).GetCustomAttribute<DescriptionAttribute>();
                return null != attribute ? attribute.Description : (defaultDescription ?? name);
            }

            return null;
        }
    }
}
#endif
