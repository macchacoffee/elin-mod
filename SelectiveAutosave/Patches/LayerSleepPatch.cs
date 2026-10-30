using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

using HarmonyLib;

using Macchacoffee.ElinMods.ModUtility.Patch;
using Macchacoffee.ElinMods.SelectiveAutosave.Mod;

namespace Macchacoffee.ElinMods.SelectiveAutosave.Patches;

[HarmonyPatch(typeof(LayerSleep))]
internal static class LayerSleepPatch
{
    private static readonly PatchTarget _patchTarget = new();

    [HarmonyPrepare]
    private static bool Prepare(MethodBase? original)
    {
        return _patchTarget.IsPatchable(original);
    }

    [HarmonyTranspiler]
    [HarmonyPatch(nameof(LayerSleep.Advance), [])]
    private static IEnumerable<CodeInstruction> Advance_Transpiler(
        IEnumerable<CodeInstruction> instructions,
        ILGenerator generator,
        MethodBase __originalMethod)
    {
        // // 変更前
        // game.Save(false, false);
        // // 変更後
        // AutosaveControl.SaveOnSleep(game, false, false);
        return AutosaveTranspiler.ReplaceSaveCalls(
            instructions,
            generator,
            __originalMethod,
            nameof(AutosaveControl.SaveOnSleep));
    }
}
