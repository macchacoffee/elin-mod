using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

using HarmonyLib;

using Macchacoffee.ElinMods.ContainerPerformanceFix.Mod;
using Macchacoffee.ElinMods.ModUtility.Logging;

namespace Macchacoffee.ElinMods.ContainerPerformanceFix.Patches;

internal static class SortNameCachePatchState
{
    internal static readonly MethodInfo? CoreSort = AccessTools.DeclaredMethod(
        typeof(CoreExtension),
        nameof(CoreExtension.Sort),
        [typeof(List<Thing>), typeof(UIList.SortMode), typeof(bool), typeof(CurrencyType)]);

    internal static readonly MethodInfo? SecondaryCompare = AccessTools.DeclaredMethod(
        typeof(Card),
        nameof(Card.SecondaryCompare),
        [typeof(UIList.SortMode), typeof(Card)]);

    private static bool _failureLogged;

    internal static bool Prepare(MethodInfo? target, string targetName)
    {
        if (target is not null)
        {
            return true;
        }

        LogFailure($"{targetName} could not be resolved.");
        return false;
    }

    internal static void LogFailure(string reason)
    {
        if (_failureLogged)
        {
            return;
        }

        _failureLogged = true;
        ModLog.Warning(
            "Failed to apply the sort-scoped GetName cache. "
            + $"The name cache optimization is disabled. Reason: {reason}");
    }
}

[HarmonyPatch]
internal static class CoreExtensionSortNameCachePatch
{
    [HarmonyPrepare]
    private static bool Prepare()
    {
        return SortNameCachePatchState.Prepare(
            SortNameCachePatchState.CoreSort,
            "CoreExtension.Sort(List<Thing>, SortMode, bool, CurrencyType)");
    }

    [HarmonyTargetMethod]
    private static MethodBase TargetMethod()
    {
        return SortNameCachePatchState.CoreSort!;
    }

    [HarmonyPrefix]
    private static void Prefix()
    {
        SortNameCache.Begin();
    }

    [HarmonyFinalizer]
    private static Exception? Finalizer(Exception? __exception)
    {
        SortNameCache.End();
        return __exception;
    }
}

[HarmonyPatch]
internal static class CardSecondaryCompareNameCachePatch
{
    [HarmonyPrepare]
    private static bool Prepare()
    {
        return SortNameCachePatchState.Prepare(
            SortNameCachePatchState.SecondaryCompare,
            "Card.SecondaryCompare(SortMode, Card)");
    }

    [HarmonyTargetMethod]
    private static MethodBase TargetMethod()
    {
        return SortNameCachePatchState.SecondaryCompare!;
    }

    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> Transpiler(
        IEnumerable<CodeInstruction> instructions)
    {
        var original = instructions.ToList();

        try
        {
            var originalGetName = AccessTools.DeclaredMethod(
                typeof(Card),
                nameof(Card.GetName),
                [typeof(NameStyle), typeof(int)]);
            var cachedGetName = AccessTools.DeclaredMethod(
                typeof(SortNameCache),
                nameof(SortNameCache.GetName),
                [typeof(Card), typeof(NameStyle), typeof(int)]);
            var comparerField = AccessTools.DeclaredField(
                typeof(Lang),
                "comparer");
            var comparerMethod = comparerField is null
                ? null
                : AccessTools.Method(
                    comparerField.FieldType,
                    nameof(StringComparer.Compare),
                    [typeof(string), typeof(string)]);

            if (originalGetName is null || cachedGetName is null
                || comparerField is null || comparerMethod is null)
            {
                SortNameCachePatchState.LogFailure(
                    "A method or field required for the SecondaryCompare matcher could not be resolved.");
                return original;
            }

            if (!TryFindNameComparison(
                original,
                originalGetName,
                comparerField,
                comparerMethod,
                out var firstGetName,
                out var secondGetName,
                out var reason))
            {
                SortNameCachePatchState.LogFailure(reason);
                return original;
            }

            // static GetName(Card, NameStyle, int) consumes the same three stack values and
            // produces the same string as the original virtual call.
            original[firstGetName].opcode = OpCodes.Call;
            original[firstGetName].operand = cachedGetName;
            original[secondGetName].opcode = OpCodes.Call;
            original[secondGetName].operand = cachedGetName;

            SortNameCache.Enable();
            return original;
        }
        catch (Exception ex)
        {
            SortNameCachePatchState.LogFailure(
                $"Unexpected error while matching Card.SecondaryCompare: {ex}");
            return original;
        }
    }

    private static bool TryFindNameComparison(
        IReadOnlyList<CodeInstruction> instructions,
        MethodInfo originalGetName,
        FieldInfo comparerField,
        MethodInfo comparerMethod,
        out int firstGetName,
        out int secondGetName,
        out string reason)
    {
        firstGetName = -1;
        secondGetName = -1;
        reason = "The expected SecondaryCompare name comparison was not found.";

        var calls = new List<int>();
        for (var i = 0; i < instructions.Count; i++)
        {
            if (instructions[i].Calls(originalGetName))
            {
                calls.Add(i);
            }
        }

        if (calls.Count != 2)
        {
            reason = $"Expected exactly two Card.GetName calls, found {calls.Count}.";
            return false;
        }

        firstGetName = calls[0];
        secondGetName = calls[1];
        if (firstGetName < 4 || secondGetName + 1 >= instructions.Count
            || secondGetName != firstGetName + 4)
        {
            reason = "The two Card.GetName calls were not in the expected adjacent comparison structure.";
            return false;
        }

        if (instructions[firstGetName - 4].opcode != OpCodes.Ldsfld
            || !Equals(instructions[firstGetName - 4].operand, comparerField)
            || instructions[firstGetName - 3].opcode != OpCodes.Ldarg_2
            || !LoadsInt32(instructions[firstGetName - 2], (int)NameStyle.Full)
            || !LoadsInt32(instructions[firstGetName - 1], 1)
            || instructions[firstGetName].opcode != OpCodes.Callvirt
            || instructions[secondGetName - 3].opcode != OpCodes.Ldarg_0
            || !LoadsInt32(instructions[secondGetName - 2], (int)NameStyle.Full)
            || !LoadsInt32(instructions[secondGetName - 1], 1)
            || instructions[secondGetName].opcode != OpCodes.Callvirt
            || !instructions[secondGetName + 1].Calls(comparerMethod))
        {
            reason = "Card.GetName calls did not use c/this with NameStyle.Full and num=1 inside Lang.comparer.Compare.";
            return false;
        }

        return true;
    }

    private static bool LoadsInt32(CodeInstruction instruction, int value)
    {
        if (instruction.opcode == OpCodes.Ldc_I4)
        {
            return instruction.operand is int intValue && intValue == value;
        }
        if (instruction.opcode == OpCodes.Ldc_I4_S)
        {
            return instruction.operand switch
            {
                sbyte sbyteValue => sbyteValue == value,
                byte byteValue => byteValue == value,
                int intValue => intValue == value,
                _ => false,
            };
        }

        return value switch
        {
            -1 => instruction.opcode == OpCodes.Ldc_I4_M1,
            0 => instruction.opcode == OpCodes.Ldc_I4_0,
            1 => instruction.opcode == OpCodes.Ldc_I4_1,
            2 => instruction.opcode == OpCodes.Ldc_I4_2,
            3 => instruction.opcode == OpCodes.Ldc_I4_3,
            4 => instruction.opcode == OpCodes.Ldc_I4_4,
            5 => instruction.opcode == OpCodes.Ldc_I4_5,
            6 => instruction.opcode == OpCodes.Ldc_I4_6,
            7 => instruction.opcode == OpCodes.Ldc_I4_7,
            8 => instruction.opcode == OpCodes.Ldc_I4_8,
            _ => false,
        };
    }
}
