using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

using HarmonyLib;

using Macchacoffee.ElinMods.ModUtility.Patch;
using Macchacoffee.ElinMods.SelectiveAutosave.Mod;

namespace Macchacoffee.ElinMods.SelectiveAutosave.Patches;

[HarmonyPatch(typeof(Chara))]
internal static class CharaPatch
{
    private static readonly PatchTarget _patchTarget = new();

    [HarmonyPrepare]
    private static bool Prepare(MethodBase? original)
    {
        return _patchTarget.IsPatchable(original);
    }

    [HarmonyTranspiler]
    [HarmonyPatch(nameof(Chara.MoveZone), [typeof(Zone), typeof(ZoneTransition)])]
    private static IEnumerable<CodeInstruction> MoveZone_Transpiler(
        IEnumerable<CodeInstruction> instructions,
        ILGenerator generator,
        MethodBase __originalMethod)
    {
        // // 変更前
        // game.Save(true, false);
        // // 変更後
        // AutosaveControl.SaveOnMapChange(game, true, false);
        return AutosaveTranspiler.ReplaceSaveCalls(
            instructions,
            generator,
            __originalMethod,
            nameof(AutosaveControl.SaveOnMapChange));
    }

    [HarmonyTranspiler]
    [HarmonyPatch(nameof(Chara.TryDropBossLoot), [])]
    private static IEnumerable<CodeInstruction> TryDropBossLoot_Transpiler(
        IEnumerable<CodeInstruction> instructions,
        ILGenerator generator,
        MethodBase __originalMethod)
    {
        // // 変更前
        // player.willAutoSave = true;
        // // 変更後
        // AutosaveControl.RequestBossAutosave(player, true);
        return AutosaveTranspiler.ReplaceRequest(
            instructions,
            generator,
            __originalMethod,
            nameof(AutosaveControl.RequestBossAutosave));
    }
}
