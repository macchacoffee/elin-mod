using System.Reflection;

using HarmonyLib;

using Macchacoffee.ElinMods.ColorfulLightSource.Mod;
using Macchacoffee.ElinMods.ModUtility.Patch;

namespace Macchacoffee.ElinMods.ColorfulLightSource.Patches;

[HarmonyPatch(typeof(Card))]
internal static class CardPatch
{
    private static readonly PatchTarget _patchTarget = new();

    [HarmonyPrepare]
    private static bool Prepare(MethodBase? original)
    {
        return _patchTarget.IsPatchable(original);
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(Card.CreateFov), [])]
    private static void CreateFov_Postfix(Card __instance, Fov __result)
    {
        LightSourceFov.UpdateEquippedLightFov(__instance, __result);
    }
}
