using System;
using System.Linq;
using System.Reflection;

namespace ModUtility.External;

public static class ModReflection
{
    public static Assembly? GetAssembly(string assemblyName)
    {
        return AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().Name == assemblyName);
    }

    public static Type? GetType(string assemblyName, string typeName)
    {
        if (GetAssembly(assemblyName) is not Assembly assembly)
        {
            return null;
        }
        return GetType(assembly, typeName);
    }

    public static Type? GetType(Assembly assembly, string typeName)
    {
        return assembly.GetType(typeName);
    }

    public static MethodInfo? GetMethod(string assemblyName, string typeName, string methodName, Type[]? parameters = null, Type[]? generics = null)
    {
        if (GetType(assemblyName, typeName) is not Type type)
        {
            return null;
        }
        return GetMethod(type, methodName, parameters, generics);
    }

    public static MethodInfo? GetMethod(Type type, string methodName, Type[]? parameters = null, Type[]? generics = null)
    {
        var flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
        if (generics is not null && type.GetMethod(methodName, generics.Length, flags, null, parameters, null) is MethodInfo method)
        {
            return method.MakeGenericMethod(generics);
        }
        return type.GetMethod(methodName, flags, null, parameters, null);
    }
}
