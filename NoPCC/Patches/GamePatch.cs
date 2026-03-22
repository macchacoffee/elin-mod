using System.Reflection;
using HarmonyLib;
using ModUtility.Patch;

namespace NoPCC.Patches;

[HarmonyPatch(typeof(Game))]
public static class GamePatch
{
    private static readonly ModPatchTarget PatchTarget = new();

    [HarmonyPrepare]
    private static bool Prepare(MethodBase? original)
    {
        return PatchTarget.IsPatchable(original);
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(Game.Load), [typeof(string), typeof(bool)])]
    private static void Load_Postfix(string id, bool cloud)
    {
        var root = cloud ? CorePath.RootSaveCloud : CorePath.RootSave + id;
        Mod.LoadConfig(root);

        // Requires to initialize after every game load.
        ModPCRenderer.Initialize();
        ModPCRenderer.Update();
    }
}
