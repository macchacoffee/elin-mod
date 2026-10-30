using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

using HarmonyLib;

using Macchacoffee.ElinMods.ModUtility.Patch;
using Macchacoffee.ElinMods.SelectiveAutosave.Mod;

namespace Macchacoffee.ElinMods.SelectiveAutosave.Patches;

[HarmonyPatch]
internal static class DramaManagerPatch
{
    private static readonly PatchTarget _patchTarget = new();

    [HarmonyPrepare]
    private static bool Prepare(MethodBase? original)
    {
        return _patchTarget.IsPatchable(original);
    }

    [HarmonyTargetMethod]
    private static MethodBase TargetMethod()
    {
        return GeneratedMethodFinder.FindGeneratedMethod(
            typeof(DramaManager),
            nameof(DramaManager.ParseLine),
            OpCodes.Callvirt,
            AutosaveTranspiler.SaveMethod);
    }

    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> ParseLineSave_Transpiler(
        IEnumerable<CodeInstruction> instructions,
        ILGenerator generator,
        MethodBase __originalMethod)
    {
        // // 変更前
        // game.Save(false, true);
        // // 変更後
        // AutosaveControl.SaveOnEvent(game, false, true);
        return AutosaveTranspiler.ReplaceSaveCalls(
            instructions,
            generator,
            __originalMethod,
            nameof(AutosaveControl.SaveOnEvent));
    }
}
