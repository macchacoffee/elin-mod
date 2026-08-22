using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Macchacoffee.ElinMods.ModUtility.External;

internal sealed class ExternalMethod
{
    public static ExternalMethodSet For(Type wrapperType) => new(wrapperType);
}

internal class ExternalMethodSet(Type wrapperType)
{
    private readonly Lazy<Type?> _externalType = new(() =>
    {
        var attr = wrapperType.GetCustomAttribute<ExternalTypeAttribute>() ??
            throw new InvalidOperationException($"{wrapperType.Name} has no ExternalTypeAttribute attribute");
        return ReflectionTools.GetType(attr.AssemblyName, attr.TypeName);
    });
    private readonly List<Func<bool>> _requiredMethodChecks = [];

    public bool IsAvailable
    {
        get
        {
            return _externalType.Value is not null && _requiredMethodChecks.All(check => check());
        }
    }

    public Lazy<TDelegate?> Create<TDelegate>(string methodName, bool required = true) where TDelegate : Delegate
    {
        var method = new Lazy<TDelegate?>(() => Resolve<TDelegate>(methodName));
        if (required)
        {
            _requiredMethodChecks.Add(() => method.Value is not null);
        }
        return method;
    }

    private TDelegate? Resolve<TDelegate>(string methodName) where TDelegate : Delegate
    {
        if (_externalType.Value is not Type type)
        {
            return null;
        }

        var delegateParams = typeof(TDelegate).GetMethod("Invoke").GetParameters();
        var delegateParamTypes = delegateParams.Select(p => p.ParameterType).ToArray();

        var method = ReflectionTools.GetMethod(type, methodName, delegateParamTypes);
        if (method is { IsStatic: true })
        {
            return DelegateFactory.CreateDelegate<TDelegate>(method);
        }

        if (delegateParamTypes.Length == 0)
        {
            return null;
        }
        var instanceMethodParamTypes = delegateParamTypes.Skip(1).ToArray();
        method = ReflectionTools.GetMethod(type, methodName, instanceMethodParamTypes);
        if (method is { IsStatic: false })
        {
            return DelegateFactory.CreateExpression<TDelegate>(method);
        }

        return null;
    }
}
