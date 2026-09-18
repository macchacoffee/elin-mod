using System.Reflection;

using HarmonyLib;

using Macchacoffee.ElinMods.ColorfulLightSource.Mod;
using Macchacoffee.ElinMods.ModUtility.Patch;

namespace Macchacoffee.ElinMods.ColorfulLightSource.Patches;

[HarmonyPatch(typeof(Game))]
internal static class GamePatch
{
    private static readonly PatchTarget _patchTarget = new();

    [HarmonyPrepare]
    private static bool Prepare(MethodBase? original)
    {
        return _patchTarget.IsPatchable(original);
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(Game.Load), [typeof(string), typeof(bool)])]
    private static void Load_Postfix()
    {
        LightSourceFov.RecalculateEquippedLightFovs();
    }
}
