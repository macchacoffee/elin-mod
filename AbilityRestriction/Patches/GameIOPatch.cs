using HarmonyLib;

namespace AbilityRestriction.Patches;

[HarmonyPatch(typeof(GameIO))]
public static class GameIOPatch
{
    [HarmonyPatch(nameof(GameIO.SaveGame), []), HarmonyPrefix]
    private static void SaveGame_Pretfix()
    {
        Mod.SaveConfig(GameIO.pathCurrentSave);
    }
}
