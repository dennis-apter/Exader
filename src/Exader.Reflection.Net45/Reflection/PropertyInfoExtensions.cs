using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Exader.Reflection
{
    public static class PropertyInfoExtensions
    {
        public static bool TryCreateGetter(this PropertyInfo property, out Func<object, object> getter)
        {
            ParameterExpression instanceParam = Expression.Parameter(typeof(object), "obj");
            UnaryExpression instance = Expression.Convert(instanceParam, property.DeclaringType);

            MethodInfo getMethod = property.GetGetMethod();
            if ((null != getMethod) && !getMethod.IsAbstract)
            {
                Expression getBody = Expression.Call(instance, getMethod);
                if (property.PropertyType.IsValueType)
                {
                    getBody = Expression.Convert(getBody, typeof(object));
                }

                Expression<Func<object, object>> getExp = Expression.Lambda<Func<object, object>>(getBody, instanceParam);
                getter = getExp.Compile();
                return true;
            }

            getter = null;
            return false;
        }

        public static bool TryCreateSetter(this PropertyInfo property, out Action<object, object> setter)
        {
            ParameterExpression instanceParam = Expression.Parameter(typeof(object), "obj");
            UnaryExpression instance = Expression.Convert(instanceParam, property.DeclaringType);
            ParameterExpression value = Expression.Parameter(typeof(object), "value");

            MethodInfo setMethod = property.GetSetMethod();
            if ((null != setMethod) && !setMethod.IsAbstract)
            {
                Expression<Action<object, object>> setExp = Expression.Lambda<Action<object, object>>(
                    Expression.Call(instance, setMethod, Expression.Convert(value, property.PropertyType)),
                    instanceParam, value);

                setter = setExp.Compile();
                return true;
            }

            setter = null;
            return false;
        }
    }
}