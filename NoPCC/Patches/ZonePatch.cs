using System.Reflection;

using HarmonyLib;

using Macchacoffee.ElinMods.ModUtility.Patch;
using Macchacoffee.ElinMods.NoPCC.Mod;

namespace Macchacoffee.ElinMods.NoPCC.Patches;

[HarmonyPatch(typeof(Zone))]
internal static class ZonePatch
{
    private static readonly PatchTarget _patchTarget = new();

    [HarmonyPrepare]
    private static bool Prepare(MethodBase? original)
    {
        return _patchTarget.IsPatchable(original);
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(Zone.Activate), [])]
    private static void Activate_Postfix()
    {
        PCRenderer.Update();
    }
}
