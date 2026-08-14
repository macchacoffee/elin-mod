using System.Reflection;
using HarmonyLib;
using ModUtility.Patch;
using NoPCC.Mod;

namespace NoPCC.Patches;

[HarmonyPatch(typeof(ConTransmute))]
internal static class ConTransmutePatch
{
    private static readonly ModPatchTarget _patchTarget = new();

    [HarmonyPrepare]
    private static bool Prepare(MethodBase? original)
    {
        return _patchTarget.IsPatchable(original);
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(ConTransmute.OnRemoved), [])]
    private static void OnRemoved_Postfix(ConTransmute __instance)
    {
        if (!__instance.owner.IsPC)
        {
            return;
        }

        // Update for preventing PCC appears after transmuting.
        ModPCRenderer.Update();
    }
}
