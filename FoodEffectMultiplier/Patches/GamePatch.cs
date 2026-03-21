using System.Reflection;
using HarmonyLib;
using ModUtility.Patch;

namespace FoodEffectMultiplier.Patches;

[HarmonyPatch(typeof(Game))]
public static class GamePatch
{
    private static readonly ModPatchTarget PatchTarget = new();

    [HarmonyPrepare]
    private static bool Prepare(MethodBase? original)
    {
        return PatchTarget.IsPatchable(original);
    }

    [HarmonyPatch(nameof(Game.Load), [typeof(string), typeof(bool)]), HarmonyPostfix]
    private static void Load_Postfix(string id, bool cloud)
    {
        Mod.LoadConfig();
    }
}
