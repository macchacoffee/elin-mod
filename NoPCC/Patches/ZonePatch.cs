using System.Reflection;
using HarmonyLib;
using ModUtility.Patch;

namespace NoPCC.Patches;

[HarmonyPatch(typeof(Zone))]
public static class ZonePatch
{
    private static readonly ModPatchTarget PatchTarget = new();

    [HarmonyPrepare]
    private static bool Prepare(MethodBase? original)
    {
        return PatchTarget.IsPatchable(original);
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(Zone.Activate), [])]
    private static void Activate_Postfix()
    {
        ModPCRenderer.Update();
    }
}
