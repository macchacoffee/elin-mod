using System.Reflection;
using HarmonyLib;
using ModUtility.Patch;
using NoPCC.Mod;

namespace NoPCC.Patches;

[HarmonyPatch(typeof(Zone))]
public static class ZonePatch
{
    private static readonly ModPatchTarget _patchTarget = new();

    [HarmonyPrepare]
    private static bool Prepare(MethodBase? original)
    {
        return _patchTarget.IsPatchable(original);
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(Zone.Activate), [])]
    private static void Activate_Postfix()
    {
        ModPCRenderer.Update();
    }
}
