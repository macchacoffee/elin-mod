using System;
using System.Linq;
using System.Reflection;

namespace ModUtility.External;

public static class ModExternalMethod
{
    public static Lazy<TDelegate?> Create<TDelegate>(Type holderType, string methodName) where TDelegate : Delegate
    {
        return new Lazy<TDelegate?>(() => Resolve<TDelegate>(holderType, methodName));
    }

    private static TDelegate? Resolve<TDelegate>(Type holderType, string methodName) where TDelegate : Delegate
    {
        var typeAttr = holderType.GetCustomAttribute<ModExternalTypeAttribute>() ??
            throw new InvalidOperationException($"{holderType.Name} has no ExternalTypeAttribute attribute");
        var delegateParams = typeof(TDelegate).GetMethod("Invoke")!.GetParameters();
        var delegateParamTypes = delegateParams.Select(p => p.ParameterType).ToArray();

        if (ModReflection.GetAssembly(typeAttr.AssemblyName) is not Assembly assembly)
        {
            return null;
        }
        if (ModReflection.GetType(assembly, typeAttr.TypeName) is not Type type)
        {
            return null;
        }

        var method = ModReflection.GetMethod(type, methodName, delegateParamTypes);
        if (method is { IsStatic: true })
        {
            return ModDelegate.AsDelegate<TDelegate>(method);
        }

        if (delegateParamTypes.Length == 0)
        {
            return null;
        }
        var instanceMethodParamTypes = delegateParamTypes.Skip(1).ToArray();
        method = ModReflection.GetMethod(type, methodName, instanceMethodParamTypes);
        if (method is { IsStatic: false })
        {
            return ModDelegate.AsExpression<TDelegate>(method);
        }

        return null;
    }
}
