using System;
using System.Reflection;
using System.Text;

namespace Exader.Reflection
{
    public static class TypeExtensions
    {
        public static StringBuilder AppendTypeName(this Type type, string ignoreNamespace, StringBuilder builder)
        {
#if NET45
            if (type.IsNullable())
            {
                Type notNullable = type.GetGenericArguments()[0];
                AppendTypeName(notNullable, ignoreNamespace, builder);
                builder.Append('?');
            }
            else if (type.IsGenericType)
            {
                string typeName = type.Name.SubstringBefore("`");
                if ((null != type.Namespace)
                    && !type.Namespace.StartsWith(ignoreNamespace))
                {
                    builder.Append(type.Namespace).Append('.');
                }

                AppendDeclaringType(builder, type);

                builder.Append(typeName).Append('<');
                bool tail = false;
                foreach (Type argument in type.GetGenericArguments())
                {
                    if (tail) builder.Append(',');
                    tail = true;

                    AppendTypeName(argument, ignoreNamespace, builder);
                }

                builder.Append('>');
            }
            else
            {
                if ((null != type.Namespace)
                    && !type.Namespace.StartsWith(ignoreNamespace))
                {
                    builder.Append(type.Namespace).Append('.');
                }

                AppendDeclaringType(builder, type);
                AppendTypeNameOrAlias(builder, type);
            }

            return builder;
#else
            var typeInfo = type.GetTypeInfo();
            if (type.IsNullable())
            {
                Type notNullable = typeInfo.GenericTypeArguments[0];
                AppendTypeName(notNullable, ignoreNamespace, builder);
                builder.Append('?');
            }
            else if (typeInfo.IsGenericType)
            {
                string typeName = type.Name.SubstringBefore("`");
                if ((null != type.Namespace)
                    && !type.Namespace.StartsWith(ignoreNamespace))
                {
                    builder.Append(type.Namespace).Append('.');
                }

                AppendDeclaringType(builder, type);

                builder.Append(typeName).Append('<');
                bool tail = false;
                foreach (Type argument in typeInfo.GenericTypeArguments)
                {
                    if (tail) builder.Append(',');
                    tail = true;

                    AppendTypeName(argument, ignoreNamespace, builder);
                }

                builder.Append('>');
            }
            else
            {
                if ((null != type.Namespace)
                    && !type.Namespace.StartsWith(ignoreNamespace))
                {
                    builder.Append(type.Namespace).Append('.');
                }

                AppendDeclaringType(builder, type);
                AppendTypeNameOrAlias(builder, type);
            }

            return builder;

#endif
        }

        public static Type GenericOf(this Type type, Type baseType)
        {
            return type.TryGetGeneric(baseType, out var genericType) ? genericType : null;
        }

        public static Type GetNonNullable(this Type type)
        {
#if NET45
            return type.IsNullable() ? type.GetGenericArguments()[0] : type;
#else
            var typeInfo = type.GetTypeInfo();
            return type.IsNullable() ? typeInfo.GenericTypeArguments[0] : type;
#endif
        }

        public static string GetTypeName(this Type type, string ignoreNamespace = "")
        {
            return AppendTypeName(type, ignoreNamespace, new StringBuilder()).ToString();
        }

        public static bool HasInterface(this Type type, Type interfaceType)
        {
#if NET45
            if (interfaceType.IsGenericTypeDefinition)
            {
                if (type.IsGenericType)
                {
                    type = type.GetGenericTypeDefinition();
                }

                if (interfaceType == type) return true;

                foreach (Type iface in type.GetInterfaces())
                {
                    if (iface.IsGenericType)
                    {
                        if (interfaceType == iface.GetGenericTypeDefinition()) return true;
                    }
                    else if (iface.IsGenericTypeDefinition)
                    {
                        if (interfaceType == iface) return true;
                    }
                }
            }
            else
            {
                foreach (Type iface in type.GetInterfaces())
                {
                    if (interfaceType == iface) return true;
                }
            }

            return false;
#else
            var typeInfo = type.GetTypeInfo();
            var interfaceTypeInfo = interfaceType.GetTypeInfo();
            if (interfaceTypeInfo.IsGenericTypeDefinition)
            {
                if (typeInfo.IsGenericType)
                {
                    type = type.GetGenericTypeDefinition();
                }

                if (interfaceType == type) return true;

                foreach (Type iface in typeInfo.ImplementedInterfaces)
                {
                    var ifaceInfo = iface.GetTypeInfo();
                    if (ifaceInfo.IsGenericType)
                    {
                        if (interfaceType == iface.GetGenericTypeDefinition()) return true;
                    }
                    else if (ifaceInfo.IsGenericTypeDefinition)
                    {
                        if (interfaceType == iface) return true;
                    }
                }
            }
            else
            {
                foreach (Type iface in typeInfo.ImplementedInterfaces)
                {
                    if (interfaceType == iface) return true;
                }
            }

            return false;
#endif
        }

        public static bool IsGenericOf(this Type type, Type baseTypeOrGenericTypeDefinition)
        {
#if NET45
            if (baseTypeOrGenericTypeDefinition.IsInterface)
            {
                return type.HasInterface(baseTypeOrGenericTypeDefinition);
            }

            return IsGenericOfInternal(type, baseTypeOrGenericTypeDefinition);
#else
            var baseTypeOrGenericTypeDefinitionInfo = baseTypeOrGenericTypeDefinition.GetTypeInfo();
            if (baseTypeOrGenericTypeDefinitionInfo.IsInterface)
            {
                return type.HasInterface(baseTypeOrGenericTypeDefinition);
            }

            return IsGenericOfInternal(type.GetTypeInfo(), baseTypeOrGenericTypeDefinitionInfo);
#endif
        }

        public static bool IsNullable(this Type type)
        {
            return (null != type) && type.GetTypeInfo().IsGenericType && (type.GetGenericTypeDefinition() == typeof(Nullable<>));
        }

        /// <summary>
        /// Finds generic type matched to specified generic type definition.
        /// </summary>
        /// <param name="genericTypeDefinition">Specified generic type definition.</param>
        /// <param name="genericType">Found generic type.</param>
        /// <returns></returns>
        public static bool TryGetGeneric(this Type type, Type genericTypeDefinition, out Type genericType)
        {
#if NET45
            if (type.IsGenericTypeDefinition)
            {
                genericType = null;
                return false;
            }

            if (!genericTypeDefinition.IsGenericTypeDefinition)
            {
                throw new ArgumentException("Base type should be a generic type definition.", nameof(genericTypeDefinition));
            }

            if (type.IsGenericType && (type.GetGenericTypeDefinition() == genericTypeDefinition))
            {
                genericType = type;
                return true;
            }

            if (genericTypeDefinition.IsInterface)
            {
                foreach (Type iface in type.GetInterfaces())
                {
                    if (!iface.IsGenericType) continue;

                    if (genericTypeDefinition == iface.GetGenericTypeDefinition())
                    {
                        genericType = iface;
                        return true;
                    }
                }

                genericType = null;
                return false;
            }

            return TryGetGenericInternal(type, genericTypeDefinition, out genericType);

#else
            var typeInfo = type.GetTypeInfo();
            var genericTypeDefinitionInfo = genericTypeDefinition.GetTypeInfo();
            if (typeInfo.IsGenericTypeDefinition)
            {
                genericType = null;
                return false;
            }

            if (!genericTypeDefinitionInfo.IsGenericTypeDefinition)
            {
                throw new ArgumentException("Base type should be a generic type definition.", nameof(genericTypeDefinition));
            }

            if (typeInfo.IsGenericType && (type.GetGenericTypeDefinition() == genericTypeDefinition))
            {
                genericType = type;
                return true;
            }

            if (genericTypeDefinitionInfo.IsInterface)
            {
                foreach (Type iface in typeInfo.ImplementedInterfaces)
                {
                    var ifaceInfo = iface.GetTypeInfo();
                    if (!ifaceInfo.IsGenericType) continue;

                    if (genericTypeDefinition == iface.GetGenericTypeDefinition())
                    {
                        genericType = iface;
                        return true;
                    }
                }

                genericType = null;
                return false;
            }

            return TryGetGenericInternal(type.GetTypeInfo(), genericTypeDefinition.GetTypeInfo(), out genericType);
#endif
        }

        private static StringBuilder AppendTypeNameOrAlias(StringBuilder builder, Type type)
        {
            if (type.Namespace == "System")
            {
                switch (type.Name)
                {
                    case "Void": return builder.Append("void");
                    case "String":
                    case "String&": // by ref
                        return builder.Append("string");
                    case "Boolean": return builder.Append("bool");
                    case "Char": return builder.Append("char");
                    case "Int32": return builder.Append("int");
                    case "Single": return builder.Append("float");
                    case "Double": return builder.Append("double");
                    case "Decimal": return builder.Append("decimal");
                    case "Int64": return builder.Append("long");
                    case "Int16": return builder.Append("short");
                    case "UInt32": return builder.Append("uint");
                    case "UInt64": return builder.Append("unlog");
                    case "UInt16": return builder.Append("ushort");
                }
            }

            return builder.Append(type.Name);
        }

        private static void AppendDeclaringType(StringBuilder builder, Type type)
        {
            if (null != type.DeclaringType && !type.IsGenericParameter)
            {
                AppendDeclaringType(builder, type.DeclaringType);
                builder.Append(type.DeclaringType.Name).Append('.');
            }
        }
        
#if NET45
        private static bool IsGenericOfInternal(Type type, Type baseType)
        {
            if (baseType.IsGenericTypeDefinition)
            {
                if (type.IsGenericType)
                {
                    type = type.GetGenericTypeDefinition();
                }
            }

            if (baseType.IsAssignableFrom(type))
            {
                return true;
            }

            Type super = type.BaseType;
            if (null == super)
            {
                return false;
            }

            return IsGenericOfInternal(super, baseType);
        }
#else
        private static bool IsGenericOfInternal(TypeInfo typeInfo, TypeInfo baseTypeInfo)
        {
            if (baseTypeInfo.IsGenericTypeDefinition)
            {
                if (typeInfo.IsGenericType)
                {
                    typeInfo = typeInfo.GetGenericTypeDefinition().GetTypeInfo();
                }
            }

            if (baseTypeInfo.IsAssignableFrom(typeInfo))
            {
                return true;
            }

            Type super = typeInfo.BaseType;
            if (null == super)
            {
                return false;
            }

            return IsGenericOfInternal(super.GetTypeInfo(), baseTypeInfo);
        }
#endif

#if NET45
        private static bool TryGetGenericInternal(Type type, Type baseType, out Type genericType)
        {
            if (type.IsGenericType)
            {
                if (baseType.IsAssignableFrom(type.GetGenericTypeDefinition()))
                {
                    genericType = type;
                    return true;
                }
            }

            Type super = type.BaseType;
            if (null == super)
            {
                genericType = null;
                return false;
            }

            return TryGetGenericInternal(super, baseType, out genericType);
        }
#else
        private static bool TryGetGenericInternal(TypeInfo typeInfo, TypeInfo baseTypeInfo, out Type genericType)
        {
            if (typeInfo.IsGenericType)
            {
                if (baseTypeInfo.IsAssignableFrom(typeInfo.GetGenericTypeDefinition().GetTypeInfo()))
                {
                    genericType = typeInfo.AsType();
                    return true;
                }
            }

            var super = typeInfo.BaseType;
            if (null == super)
            {
                genericType = null;
                return false;
            }

            return TryGetGenericInternal(super.GetTypeInfo(), baseTypeInfo, out genericType);
        }
#endif
    }
}
