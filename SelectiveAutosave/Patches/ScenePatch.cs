using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

using HarmonyLib;

using Macchacoffee.ElinMods.ModUtility.Patch;
using Macchacoffee.ElinMods.SelectiveAutosave.Mod;

namespace Macchacoffee.ElinMods.SelectiveAutosave.Patches;

[HarmonyPatch(typeof(Scene))]
internal static class ScenePatch
{
    private static readonly PatchTarget _patchTarget = new();

    [HarmonyPrepare]
    private static bool Prepare(MethodBase? original)
    {
        return _patchTarget.IsPatchable(original);
    }

    [HarmonyTranspiler]
    [HarmonyPatch(nameof(Scene.Init), [typeof(Scene.Mode)])]
    private static IEnumerable<CodeInstruction> Init_Transpiler(
        IEnumerable<CodeInstruction> instructions,
        ILGenerator generator,
        MethodBase __originalMethod)
    {
        // // 変更前
        // game.Save(false, false);
        // // 変更後
        // AutosaveControl.SaveOnMapChange(game, false, false);
        return AutosaveTranspiler.ReplaceSaveCalls(
            instructions,
            generator,
            __originalMethod,
            nameof(AutosaveControl.SaveOnMapChange));
    }
}
