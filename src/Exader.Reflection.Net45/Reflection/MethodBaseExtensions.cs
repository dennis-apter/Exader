using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Exader.Reflection
{
    public static class MethodBaseExtensions
    {
        public static string GetMethodName(this MethodBase method, string ignoreNamespace = "")
        {
            bool tail;

            var buffer = new StringBuilder();

            var methodInfo = method as MethodInfo;
            if (methodInfo != null)
            {
                buffer.Append(methodInfo.Name);
                if (methodInfo.IsGenericMethodDefinition)
                {
                    buffer.Append("<");
                    tail = false;
                    foreach (Type genericArgument in methodInfo.GetGenericArguments())
                    {
                        if (tail) { buffer.Append(", "); }
                        tail = true;

                        buffer.Append(genericArgument.Name);
                    }

                    buffer.Append(">");
                }
            }
            else
            {
                buffer.Append("new");
            }

            buffer.Append("(");

            tail = false;
            foreach (ParameterInfo parameter in method.GetParameters())
            {
                if (tail) { buffer.Append(", "); }
                tail = true;

                if (parameter.IsOut)
                {
                    buffer.Append("out ");
                }

                parameter.ParameterType.AppendTypeName(ignoreNamespace, buffer);
            }

            buffer.Append(")");

            methodInfo = method as MethodInfo;
            if (methodInfo != null)
            {
                if (typeof(void) != methodInfo.ReturnType)
                {
                    buffer.Append(" : ");
                    methodInfo.ReturnType.AppendTypeName(ignoreNamespace, buffer);
                }
            }

            return buffer.ToString();
        }

        public static IEnumerable<MemberInfo> GetUsedMembers(this MethodBase method, bool throwsOnError = false)
        {
            if (method.IsAbstract)
            {
                if (throwsOnError)
                {
                    throw new ArgumentException(
                        string.Format("Abstract method “{0}” cannot be analyzed.", method));
                }

                return null;
            }

            return new MethodBodyReader(method).ReadUsedMembers(throwsOnError).Distinct();
        }

        public static IEnumerable<MemberInfo> GetUsedOwnMembers(this MethodBase method, bool throwsOnError = false)
        {
            if (method.DeclaringType == null)
            {
                throw new ArgumentException(
                    string.Format("Method “{0}” does not has declaring type.", method));
            }

            var members = GetUsedMembers(method, throwsOnError);
            return members.Where(m => m.DeclaringType != null && m.DeclaringType.IsAssignableFrom(method.DeclaringType));
        }

        public static IEnumerable<PropertyInfo> GetUsedOwnProperties(this MethodBase method, bool throwsOnError = false)
        {
            if (method.DeclaringType == null)
            {
                throw new ArgumentException(
                    string.Format("Method “{0}” does not has declaring type.", method));
            }

            var properties = GetUsedProperties(method, throwsOnError);
            return properties.Where(p => p.DeclaringType != null && p.DeclaringType.IsAssignableFrom(method.DeclaringType));
        }

        public static IEnumerable<PropertyInfo> GetUsedProperties(this MethodBase method, bool throwsOnError = false)
        {
            foreach (MemberInfo usedMember in method.GetUsedMembers(throwsOnError))
            {
                var m = usedMember as MethodInfo;
                if (m == null || m.DeclaringType == null || !m.IsSpecialName) continue;
                if (!m.Name.StartsWith("get_") && !m.Name.StartsWith("set_")) continue;

                string name = m.Name.Substring(4);
                PropertyInfo result = m.DeclaringType.GetProperty(name);
                if (null != result)
                {
                    yield return result;
                }
            }
        }
    }
}
