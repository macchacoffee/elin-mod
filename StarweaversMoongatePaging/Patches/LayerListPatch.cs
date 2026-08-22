using System.Reflection;

using HarmonyLib;

using Macchacoffee.ElinMods.ModUtility.Patch;
using Macchacoffee.ElinMods.StarweaversMoongatePaging.Mod;

namespace Macchacoffee.ElinMods.StarweaversMoongatePaging.Patches;

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
