using System.Reflection;
using HarmonyLib;
using Macchacoffee.ElinMods.ModUtility.Patch;

namespace Macchacoffee.ElinMods.NoPCC.Patches;

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
    private static void SaveGame_Pretfix()
    {
        ModContext.SaveConfig(GameIO.pathCurrentSave);
    }
}
