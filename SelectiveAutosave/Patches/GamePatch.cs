using System.Reflection;

using HarmonyLib;

using Macchacoffee.ElinMods.ModUtility.Patch;

namespace Macchacoffee.ElinMods.SelectiveAutosave.Patches;

[HarmonyPatch(typeof(Game))]
internal static class GamePatch
{
    private static readonly PatchTarget _patchTarget = new();

    [HarmonyPrepare]
    private static bool Prepare(MethodBase? original)
    {
        return _patchTarget.IsPatchable(original);
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(Game.GotoTitle), [typeof(bool)])]
    private static bool GotoTitle_Prefix(bool showDialog)
    {
        if (!showDialog || ModContext.Config.SaveOnReturnToTitle.Value)
        {
            return true;
        }
        Dialog.YesNo(
            ModConsts.SourceId.ConfirmReturnToTitle,
            () => EClass.scene.Init(Scene.Mode.Title));
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(Game.Quit), [])]
    private static bool Quit_Prefix()
    {
        if (ModContext.Config.SaveOnQuit.Value)
        {
            return true;
        }
        Dialog.YesNo(
            ModConsts.SourceId.ConfirmQuit,
            () => EClass.core.Quit());
        return false;
    }
}
