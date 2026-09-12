using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

using HarmonyLib;

using Macchacoffee.ElinMods.ContainerPerformanceFix.Mod;
using Macchacoffee.ElinMods.ModUtility.Logging;
using Macchacoffee.ElinMods.ModUtility.Patch;

namespace Macchacoffee.ElinMods.ContainerPerformanceFix.Patches;

[HarmonyPatch(typeof(Card))]
internal static class CardPatch
{
    private static readonly PatchTarget _patchTarget = new();

    [HarmonyPrepare]
    private static bool Prepare(MethodBase? original)
    {
        return _patchTarget.IsPatchable(original);
    }

    private static bool _secondaryCompareFailureLogged;

    [HarmonyTranspiler]
    [HarmonyPatch(
        nameof(Card.SecondaryCompare),
        [typeof(UIList.SortMode), typeof(Card)])]
    private static IEnumerable<CodeInstruction> SecondaryCompare_Transpiler(
        IEnumerable<CodeInstruction> instructions,
        ILGenerator generator)
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
                throw new MissingMemberException(
                    "A method or field required for the SecondaryCompare matcher could not be resolved.");
            }

            // // 変更前
            // Lang.comparer.Compare(c.GetName(NameStyle.Full, 1), GetName(NameStyle.Full, 1));
            // // 変更後
            // Lang.comparer.Compare(
            //     SortNameCache.GetName(c, NameStyle.Full, 1),
            //     SortNameCache.GetName(this, NameStyle.Full, 1));
            var matcher = new CodeMatcher(original, generator);

            matcher.MatchStartForward(
                new CodeMatch(OpCodes.Ldsfld, comparerField),
                new CodeMatch(OpCodes.Ldarg_2),
                new CodeMatch(instruction => instruction.LoadsConstant(NameStyle.Full)),
                new CodeMatch(instruction => instruction.LoadsConstant(1)),
                new CodeMatch(OpCodes.Callvirt, originalGetName),
                new CodeMatch(OpCodes.Ldarg_0),
                new CodeMatch(instruction => instruction.LoadsConstant(NameStyle.Full)),
                new CodeMatch(instruction => instruction.LoadsConstant(1)),
                new CodeMatch(OpCodes.Callvirt, originalGetName),
                new CodeMatch(OpCodes.Callvirt, comparerMethod)
            ).ThrowIfInvalid(
                "Could not find Card.SecondaryCompare's name comparison.");

            // static GetName(Card, NameStyle, int) consumes the same three stack values and
            // produces the same string as the original virtual call.
            matcher.Advance(4)
                .SetAndAdvance(OpCodes.Call, cachedGetName)
                .Advance(3)
                .Set(OpCodes.Call, cachedGetName);

            SortNameCache.Enable();
            return matcher.InstructionEnumeration();
        }
        catch (Exception ex)
        {
            LogSecondaryComparePatchFailure(
                $"Unexpected error while matching Card.SecondaryCompare: {ex}");
            return original;
        }
    }

    private static void LogSecondaryComparePatchFailure(string reason)
    {
        if (_secondaryCompareFailureLogged)
        {
            return;
        }

        _secondaryCompareFailureLogged = true;
        ModLog.Warning(
            "Failed to patch Card.SecondaryCompare for the sort-scoped GetName cache. "
            + $"Falling back to vanilla name lookup. Reason: {reason}");
    }
}
