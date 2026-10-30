using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

using HarmonyLib;

namespace Macchacoffee.ElinMods.ModUtility.Patch;

internal static class GeneratedMethodFinder
{
    public static MethodBase FindGeneratedMethod(
        Type owner,
        string methodName,
        OpCode opcode,
        MemberInfo member)
    {
        MethodInfo? target = null;
        var prefix = $"<{methodName}>";

        foreach (var type in EnumerateTypes(owner))
        {
            foreach (var method in AccessTools.GetDeclaredMethods(type))
            {
                if (!method.Name.StartsWith(prefix, StringComparison.Ordinal) || method.GetMethodBody() is null)
                {
                    continue;
                }
                foreach (var instruction in PatchProcessor.ReadMethodBody(method))
                {
                    if (instruction.Key != opcode || !Equals(instruction.Value, member))
                    {
                        continue;
                    }
                    if (target is not null)
                    {
                        throw new InvalidOperationException($"Multiple generated targets found for {owner.FullName}.{methodName} ({member.Name}).");
                    }
                    target = method;
                    break;
                }
            }
        }
        return target ?? throw new MissingMethodException($"Could not find the generated target for {owner.FullName}.{methodName} ({member.Name}).");
    }

    private static IEnumerable<Type> EnumerateTypes(Type owner)
    {
        yield return owner;
        foreach (var nested in owner.GetNestedTypes(BindingFlags.Public | BindingFlags.NonPublic))
        {
            foreach (var type in EnumerateTypes(nested))
            {
                yield return type;
            }
        }
    }
}
