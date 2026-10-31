using System.Reflection;

using HarmonyLib;

using Macchacoffee.ElinMods.AdditionalUniqueNPC.Mod;
using Macchacoffee.ElinMods.ModUtility.Patch;

namespace Macchacoffee.ElinMods.AdditionalUniqueNPC.Patches;

[HarmonyPatch(typeof(Chara))]
internal static class CharaPatch
{
    private static readonly PatchTarget _patchTarget = new();

    [HarmonyPrepare]
    private static bool Prepare(MethodBase? original)
    {
        return _patchTarget.IsPatchable(original);
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(Chara.OnBanish), [])]
    private static void OnBanish_Prefix(Chara __instance)
    {
        FixedUniqueCharaPlacement.EnsureReturnLocation(__instance);
    }
}
