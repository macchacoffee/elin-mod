using System.Reflection;
using HarmonyLib;

namespace AbilityRestriction.Patches;

[HarmonyPatch(typeof(Game))]
public static class GamePatch
{
    private static readonly PatchTarget Target = new();

    [HarmonyPrepare]
    private static bool Prepare(MethodBase? original)
    {
        return Target.IsPatchable(original);
    }

    [HarmonyPatch(nameof(Game.Load), [typeof(string), typeof(bool)]), HarmonyPostfix]
    private static void Load_Postfix(string id, bool cloud)
    {
        var root = cloud ? CorePath.RootSaveCloud : CorePath.RootSave + id;
        Mod.LoadConfig(root);
    }
}
