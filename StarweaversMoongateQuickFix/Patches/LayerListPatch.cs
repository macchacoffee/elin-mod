using System;
using System.Collections.Generic;
using System.Reflection;

using HarmonyLib;

using Macchacoffee.ElinMods.ModUtility.Patch;
using Macchacoffee.ElinMods.StarweaversMoongateQuickFix.Mod;

namespace Macchacoffee.ElinMods.StarweaversMoongateQuickFix.Patches;

[HarmonyPatch(typeof(LayerList))]
internal static class LayerListPatch
{
    private static readonly PatchTarget _patchTarget = new();

    [HarmonyPrepare]
    private static bool Prepare(MethodBase? original)
    {
        return _patchTarget.IsPatchable(original);
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(LayerList.OnKill), [])]
    private static void OnKill_Prefix(LayerList __instance)
    {
        MoongatePaging.Detach(__instance);
    }
}

[HarmonyPatch]
internal static class LayerListSetList2Patch
{
    private static readonly PatchTarget _patchTarget = new();

    [HarmonyPrepare]
    private static bool Prepare(MethodBase? original)
    {
        return _patchTarget.IsPatchable(original);
    }

    [HarmonyTargetMethod]
    private static MethodBase SetList2_TargetMethod()
    {
        return AccessTools.DeclaredMethod(
                   typeof(LayerList),
                   nameof(LayerList.SetList2),
                   generics: [typeof(MapMetaData)])
               ?? throw new MissingMethodException(typeof(LayerList).FullName, nameof(LayerList.SetList2));
    }

    [HarmonyPrefix]
    private static void Prefix(LayerList __instance, ref ICollection<MapMetaData> __0)
    {
        if (!MoongatePaging.IsOpening)
        {
            return;
        }
        if (__0 is not List<MapMetaData> source)
        {
            return;
        }

        MoongatePaging.Attach(__instance, source, ref __0);
    }

    [HarmonyPostfix]
    private static void Postfix()
    {
        if (!MoongatePaging.IsOpening)
        {
            return;
        }

        MoongatePaging.SetupPageControls();
    }
}
