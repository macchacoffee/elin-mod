using System.Reflection;
using HarmonyLib;
using ModUtility.Patch;

namespace AddPalmiaTimesNewsToLog.Patches;

[HarmonyPatch(typeof(GameIO))]
public static class GameIOPatch
{
    private static readonly ModPatchTarget PatchTarget = new();

    [HarmonyPrepare]
    private static bool Prepare(MethodBase? original)
    {
        return PatchTarget.IsPatchable(original);
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(GameIO.SaveGame), [])]
    private static void SaveGame_Prefix()
    {
        Mod.SaveConfig(GameIO.pathCurrentSave);
    }
}
