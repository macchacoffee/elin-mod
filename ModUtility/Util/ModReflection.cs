using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using HarmonyLib;

namespace ModUtility.Util;

public static class ModReflection
{
    public static Assembly? FindAssembly(string assemblyName)
    {
        var sharedData = GetSharedData<Assembly?>(nameof(FindAssembly));
        return sharedData.GetOrAdd(assemblyName, (_) => AccessTools.AllAssemblies().FirstOrDefault(a => a.GetName().Name == assemblyName));
    }

    public static Type? FindType(string assemblyName, string namespaceName, string typeName)
    {
        if (FindAssembly(assemblyName) is not Assembly assembly)
        {
            return null;
        }
        return FindType(assembly, namespaceName, typeName);
    }

    public static Type? FindType(Assembly assembly, string namespaceName, string typeName)
    {
        var sharedData = GetSharedData<Type?>(nameof(FindType));
        var key = $"{assembly.GetName()}.{namespaceName}.{typeName}";
        return sharedData.GetOrAdd(key, (_) => AccessTools.GetTypesFromAssembly(assembly).FirstOrDefault(t => t.Namespace == namespaceName && t.Name == typeName));
    }

    public static IModMethodDelegate? FindMethod(string assemblyName, string namespaceName, string typeName, string methodName, Type[]? parameters = null, Type[]? generics = null)
    {
        if (FindType(assemblyName, namespaceName, typeName) is not Type type)
        {
            return null;
        }
        return FindMethod(type, methodName, parameters, generics);
    }

    public static IModMethodDelegate? FindMethod(Type type, string methodName, Type[]? parameters = null, Type[]? generics = null)
    {
        var sharedData = GetSharedData<IModMethodDelegate?>(nameof(FindMethod));
        var key = $"{type.Assembly.GetName()}.{type.Namespace}{type.Name}.{methodName}";
        return sharedData.GetOrAdd(key, (_) => AccessTools.Method(type, methodName, parameters, generics) is MethodInfo method ? CreateMethodDelegate(type, method) : null);
    }

    private static IModMethodDelegate CreateMethodDelegate(Type type, MethodInfo method)
    {
        var args = Expression.Parameter(typeof(object?[]));
        var (instance, indexOffset) = method.IsStatic ? (null, 0) : (Expression.Convert(Expression.ArrayIndex(args, Expression.Constant(0)), type), 1);
        var parameters = method.GetParameters().Select(
            (x, i) => Expression.Convert(Expression.ArrayIndex(args, Expression.Constant(i + indexOffset)), x.ParameterType))
            .ToArray();
        var call = Expression.Call(instance, method, parameters);
        if (method.ReturnType == typeof(void))
        {
            return new ModActionMethodDelegate(
                Expression.Lambda<Action<object?[]>>(call, args).Compile());
        }
        return new ModFuncMethodDelegate(
            Expression.Lambda<Func<object?[], object?>>(Expression.Convert(call, typeof(object)), args).Compile());
    }

    private static ConcurrentDictionary<string, T> GetSharedData<T>(string key)
    {
        return ModSharedStore.GetData<string, T>($"{nameof(Util)}.{nameof(ModReflection)}.{key}");
    }
}

public interface IModMethodDelegate
{
    public ModMethodResult Invoke(params object?[] args);
}

public class ModActionMethodDelegate(Action<object?[]> method) : IModMethodDelegate
{
    private Action<object?[]> Method { get; } = method;

    public ModMethodResult Invoke(params object?[] args)
    {
        Method(args);
        return new();
    }
}

public class ModFuncMethodDelegate(Func<object?[], object?> method) : IModMethodDelegate
{
    private Func<object?[], object?> Method { get; } = method;
    public ModMethodResult Invoke(params object?[] args)
    {
        return new(Method(args));
    }
}

public class ModMethodResult
{
    public bool IsEmpty { get; }
    public object? Value { get; }

    public ModMethodResult()
    {
        IsEmpty = true;
    }

    public ModMethodResult(object? value)
    {
        IsEmpty = false;
        Value = value;
    }

    public void WithValue(Action<object?> action)
    {
        if (!IsEmpty)
        {
            action(Value);
        }
    }
}
