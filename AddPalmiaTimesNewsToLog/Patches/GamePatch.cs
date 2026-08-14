using System.Reflection;
using HarmonyLib;
using ModUtility.Patch;

namespace AddPalmiaTimesNewsToLog.Patches;

[HarmonyPatch(typeof(Game))]
internal static class GamePatch
{
    private static readonly ModPatchTarget _patchTarget = new();

    [HarmonyPrepare]
    private static bool Prepare(MethodBase? original)
    {
        return _patchTarget.IsPatchable(original);
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(Game.Load), [typeof(string), typeof(bool)])]
    private static void Load_Postfix(string id, bool cloud)
    {
        var root = (cloud ? CorePath.RootSaveCloud : CorePath.RootSave) + id;
        ModContext.LoadConfig(root);
    }
}
