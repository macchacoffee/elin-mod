using System.Reflection;
using HarmonyLib;
using ModUtility.Patch;

namespace SomewhatEnhancedDisplay.Patches;

[HarmonyPatch(typeof(GameIO))]
internal static class GameIOPatch
{
    private static readonly ModPatchTarget _patchTarget = new();

    [HarmonyPrepare]
    private static bool Prepare(MethodBase? original)
    {
        return _patchTarget.IsPatchable(original);
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(GameIO.SaveGame), [])]
    private static void SaveGame_Prefix()
    {
        ModContext.SaveWorldConfig(GameIO.pathCurrentSave);
    }
}
