using System;
using System.Reflection;
using HarmonyLib;
using Macchacoffee.ElinMods.ModUtility.Patch;
using Macchacoffee.ElinMods.StarweaversMoongateQuickFix.Mod;

namespace Macchacoffee.ElinMods.StarweaversMoongateQuickFix.Patches;

[HarmonyPatch(typeof(TraitMoongateEx))]
internal static class TraitMoongateExPatch
{
    private static readonly ModPatchTarget _patchTarget = new();

    [HarmonyPrepare]
    private static bool Prepare(MethodBase? original)
    {
        return _patchTarget.IsPatchable(original);
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(TraitMoongateEx._OnUse), [])]
    private static void _OnUse_Prefix(TraitMoongateEx __instance)
    {
        ModMoongatePaging.IsOpening = true;
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(TraitMoongateEx._OnUse), [])]
    private static void _OnUse_Postfix(TraitMoongateEx __instance)
    {
        ModMoongatePaging.IsOpening = false;
    }

    [HarmonyFinalizer]
    [HarmonyPatch(nameof(TraitMoongateEx._OnUse), [])]
    private static Exception _OnUse_Finalizer(Exception __exception)
    {
        ModMoongatePaging.IsOpening = false;
        return __exception;
    }
}
