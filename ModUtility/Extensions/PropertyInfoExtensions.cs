using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace ModUtility.Extensions;

internal static class PropertyInfoExtensions
{
    private static readonly Dictionary<PropertyInfo, object> PropertyGetters = [];

    public static Func<TClass, TValue> GetGetter<TClass, TValue>(this PropertyInfo propInfo)
    {
        if (!PropertyGetters.TryGetValue(propInfo, out var getter))
        {
            var args = Expression.Parameter(typeof(TClass));
            var body = Expression.Property(args, propInfo);
            getter = Expression.Lambda<Func<TClass, TValue>>(body, args).Compile();
            PropertyGetters[propInfo] = getter;
        }
        return (getter as Func<TClass, TValue>)!;
    }
}
