using System.Reflection;
using HarmonyLib;
using Macchacoffee.ElinMods.ModUtility.Patch;
using Macchacoffee.ElinMods.NoPCC.Mod;

namespace Macchacoffee.ElinMods.NoPCC.Patches;

[HarmonyPatch(typeof(Chara))]
internal static class CharaPatch
{
    private static readonly ModPatchTarget _patchTarget = new();

    [HarmonyPrepare]
    private static bool Prepare(MethodBase? original)
    {
        return _patchTarget.IsPatchable(original);
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
