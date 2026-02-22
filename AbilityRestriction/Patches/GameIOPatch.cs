using System.Reflection;
using HarmonyLib;

namespace AbilityRestriction.Patches;

[HarmonyPatch(typeof(GameIO))]
public static class GameIOPatch
{
    private static readonly PatchTarget Target = new();

    [HarmonyPrepare]
    private static bool Prepare(MethodBase? original)
    {
        return Target.IsPatchable(original);
    }

    [HarmonyPatch(nameof(GameIO.SaveGame), []), HarmonyPrefix]
    private static void SaveGame_Prefix()
    {
        Mod.SaveConfig(GameIO.pathCurrentSave);
    }
}
