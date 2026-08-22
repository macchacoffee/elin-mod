using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Macchacoffee.ElinMods.ModUtility.External;

internal static class DelegateFactory
{
    public static TDelegate CreateDelegate<TDelegate>(MethodInfo method) where TDelegate : Delegate
    {
        return method.CreateDelegate<TDelegate>();
    }

    public static TDelegate CreateExpression<TDelegate>(MethodInfo method) where TDelegate : Delegate
    {
        var declaringType = method.DeclaringType;
        var invokeMethod = typeof(TDelegate).GetMethod(nameof(Action.Invoke));
        var delegateParams = invokeMethod.GetParameters();
        var methodParams = method.GetParameters();

        var parameters = delegateParams
            .Select((p, i) => Expression.Parameter(p.ParameterType, p.Name ?? $"arg{i}"))
            .ToArray();

        var (instance, indexOffset) = method.IsStatic
            ? (null, 0)
            : (Expression.Convert(parameters[0], declaringType), 1);

        var methodArgs = methodParams
            .Select((p, i) =>
            {
                var arg = parameters[i + indexOffset];
                return arg.Type == p.ParameterType ? (Expression)arg : Expression.Convert(arg, p.ParameterType);
            })
            .ToArray();

        Expression body = Expression.Call(instance, method, methodArgs);
        if (invokeMethod.ReturnType == typeof(void) && body.Type != invokeMethod.ReturnType)
        {
            body = Expression.Convert(body, invokeMethod.ReturnType);
        }

        return Expression.Lambda<TDelegate>(body, parameters).Compile();
    }
}
