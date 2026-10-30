using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

using HarmonyLib;

using Macchacoffee.ElinMods.ModUtility.Patch;
using Macchacoffee.ElinMods.SelectiveAutosave.Mod;

namespace Macchacoffee.ElinMods.SelectiveAutosave.Patches;

[HarmonyPatch]
internal static class TraitDeedRelocatePatch
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
            typeof(TraitDeedRelocate),
            nameof(TraitDeedRelocate.OnRead),
            OpCodes.Callvirt,
            AutosaveTranspiler.SaveMethod);
    }

    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> OnReadConfirm_Transpiler(
        IEnumerable<CodeInstruction> instructions,
        ILGenerator generator,
        MethodBase __originalMethod)
    {
        // // 変更前
        // game.Save(false, false);
        // // 変更後
        // AutosaveControl.SaveOnRelocation(game, false, false);
        return AutosaveTranspiler.ReplaceSaveCalls(
            instructions,
            generator,
            __originalMethod,
            nameof(AutosaveControl.SaveOnRelocation));
    }
}
