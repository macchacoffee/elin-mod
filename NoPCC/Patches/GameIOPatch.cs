using System.Reflection;
using HarmonyLib;
using ModUtility.Patch;

namespace NoPCC.Patches;

[HarmonyPatch(typeof(GameIO))]
public static class GameIOPatch
{
    private static readonly ModPatchTarget _patchTarget = new();

    [HarmonyPrepare]
    private static bool Prepare(MethodBase? original)
    {
        return _patchTarget.IsPatchable(original);
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(GameIO.SaveGame), [])]
    private static void SaveGame_Pretfix()
    {
        ModContext.SaveConfig(GameIO.pathCurrentSave);
    }
}
