using System.Reflection;

using HarmonyLib;

using Macchacoffee.ElinMods.ColorfulLightSource.Mod;
using Macchacoffee.ElinMods.ModUtility.Patch;

namespace Macchacoffee.ElinMods.ColorfulLightSource.Patches;

[HarmonyPatch(typeof(GameDate))]
internal static class GameDatePatch
{
    private static readonly PatchTarget _patchTarget = new();

    [HarmonyPrepare]
    private static bool Prepare(MethodBase? original)
    {
        return _patchTarget.IsPatchable(original);
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(GameDate.AdvanceHour), [])]
    private static void AdvanceHour_Postfix()
    {
        LightSourceFov.RecalculateEquippedLightFovs();
    }
}
