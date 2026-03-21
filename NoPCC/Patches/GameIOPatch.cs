using System.Reflection;
using HarmonyLib;
using ModUtility.Patch;

namespace NoPCC.Patches;

[HarmonyPatch(typeof(GameIO))]
public static class GameIOPatch
{
    private static readonly ModPatchTarget PatchTarget = new();

    [HarmonyPrepare]
    private static bool Prepare(MethodBase? original)
    {
        return PatchTarget.IsPatchable(original);
    }

    [HarmonyPatch(nameof(GameIO.SaveGame), []), HarmonyPrefix]
    private static void SaveGame_Pretfix()
    {
        Mod.SaveConfig(GameIO.pathCurrentSave);
    }
}
