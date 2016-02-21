//#if !NET456
using System;
using System.ComponentModel;
using System.Reflection;

namespace Exader.Reflection
{
    public static class CustomAttributeProviderExtensions
    {
        public static T GetCustomAttribute<T>(this ICustomAttributeProvider self) where T : Attribute
        {
            return self.GetCustomAttribute<T>(false);
        }

        public static T GetCustomAttribute<T>(this ICustomAttributeProvider self, bool inherit) where T : Attribute
        {
            object[] attributes = self.GetCustomAttributes(typeof(T), inherit);
            if (0 == attributes.Length)
            {
                return null;
            }

            if (1 < attributes.Length)
            {
                throw new InvalidOperationException(
                    $"Элемент {self} содержит более одного атрибута {typeof (T).FullName}.");
            }

            return (T)attributes[0];
        }

        public static T[] GetCustomAttributes<T>(this ICustomAttributeProvider self) where T : Attribute
        {
            return (T[])self.GetCustomAttributes(typeof(T), false);
        }

        public static T[] GetCustomAttributes<T>(this ICustomAttributeProvider self, bool inherit) where T : Attribute
        {
            return (T[])self.GetCustomAttributes(typeof(T), inherit);
        }

        public static string GetDescription(this ICustomAttributeProvider self)
        {
            var attribute = self.GetCustomAttribute<DescriptionAttribute>();
            return null == attribute ? null : attribute.Description;
        }

        public static bool HasCustomAttribute<T>(this ICustomAttributeProvider self, bool inherit) where T : Attribute
        {
            return null != self.GetCustomAttribute<T>(inherit);
        }

        public static bool HasCustomAttribute<T>(this ICustomAttributeProvider self) where T : Attribute
        {
            return null != self.GetCustomAttribute<T>(false);
        }
    }
}
//#endif
