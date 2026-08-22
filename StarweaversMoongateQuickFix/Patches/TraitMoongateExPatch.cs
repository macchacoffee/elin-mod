using System;
using System.Reflection;

using HarmonyLib;

using Macchacoffee.ElinMods.ModUtility.Patch;
using Macchacoffee.ElinMods.StarweaversMoongateQuickFix.Mod;

namespace Macchacoffee.ElinMods.StarweaversMoongateQuickFix.Patches;

[HarmonyPatch(typeof(TraitMoongateEx))]
internal static class TraitMoongateExPatch
{
    private static readonly PatchTarget _patchTarget = new();

    [HarmonyPrepare]
    private static bool Prepare(MethodBase? original)
    {
        return _patchTarget.IsPatchable(original);
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(TraitMoongateEx._OnUse), [])]
    private static void _OnUse_Prefix()
    {
        MoongatePaging.IsOpening = true;
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(TraitMoongateEx._OnUse), [])]
    private static void _OnUse_Postfix()
    {
        MoongatePaging.IsOpening = false;
    }

    [HarmonyFinalizer]
    [HarmonyPatch(nameof(TraitMoongateEx._OnUse), [])]
    private static Exception? _OnUse_Finalizer(Exception? __exception)
    {
        MoongatePaging.IsOpening = false;
        if (__exception != null)
        {
            MoongatePaging.AbortOpening();
        }
        return __exception;
    }
}
