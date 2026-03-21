using System.Reflection;
using HarmonyLib;
using ModUtility.Patch;

namespace NoPCC.Patches;

[HarmonyPatch(typeof(ConTransmute))]
public static class ConTransmutePatch
{
    private static readonly ModPatchTarget PatchTarget = new();

    [HarmonyPrepare]
    private static bool Prepare(MethodBase? original)
    {
        return PatchTarget.IsPatchable(original);
    }

    [HarmonyPatch(nameof(ConTransmute.OnRemoved), []), HarmonyPostfix]
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
