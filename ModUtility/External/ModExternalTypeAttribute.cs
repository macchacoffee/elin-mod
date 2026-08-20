using System;

namespace Macchacoffee.ElinMods.ModUtility.External;

[AttributeUsage(AttributeTargets.Class)]
public sealed class ModExternalTypeAttribute(string assemblyName, string typeName) : Attribute
{
    public string AssemblyName { get; } = assemblyName;
    public string TypeName { get; } = typeName;
}
