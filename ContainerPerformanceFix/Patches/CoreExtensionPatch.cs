using System;
using System.Collections.Generic;
using System.Reflection;

using HarmonyLib;

using Macchacoffee.ElinMods.ContainerPerformanceFix.Mod;
using Macchacoffee.ElinMods.ModUtility.Patch;

namespace Macchacoffee.ElinMods.ContainerPerformanceFix.Patches;

[HarmonyPatch(typeof(CoreExtension))]
internal static class CoreExtensionPatch
{
    private static readonly PatchTarget _patchTarget = new();

    [HarmonyPrepare]
    private static bool Prepare(MethodBase? original)
    {
        return _patchTarget.IsPatchable(original);
    }

    [HarmonyPrefix]
    [HarmonyPatch(
        nameof(CoreExtension.Sort),
        [typeof(List<Thing>), typeof(UIList.SortMode), typeof(bool), typeof(CurrencyType)])]
    private static void Sort_Prefix()
    {
        SortNameCache.Begin();
    }

    [HarmonyFinalizer]
    [HarmonyPatch(
        nameof(CoreExtension.Sort),
        [typeof(List<Thing>), typeof(UIList.SortMode), typeof(bool), typeof(CurrencyType)])]
    private static Exception? Sort_Finalizer(Exception? __exception)
    {
        SortNameCache.End();
        return __exception;
    }
}
