using System.Reflection;
using HarmonyLib;
using ModUtility.Patch;

namespace NoPCC.Patches;

[HarmonyPatch(typeof(Chara))]
public static class CharaPatch
{
    private static readonly ModPatchTarget PatchTarget = new();

    [HarmonyPrepare]
    private static bool Prepare(MethodBase? original)
    {
        return PatchTarget.IsPatchable(original);
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(Chara.Tick), [])]
    private static void Tick_Prefix(Chara __instance)
    {
        if (!__instance.IsPC)
        {
            return;
        }

        ModPCRenderer.Update();
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(Chara.SetPCCState), [typeof(PCCState)])]
    private static void PCCState_Postfix(Chara __instance)
    {
        if (!__instance.IsPC)
        {
            return;
        }

        ModPCRenderer.Update();
    }
}
