using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using ModUtility.Patch;

namespace SomewhatEnhancedDisplay.Patches;

[HarmonyPatch(typeof(NotificationBuff))]
public static class NotificationBuffPatch
{
    private static readonly ModPatchTarget PatchTarget = new();

    [HarmonyPrepare]
    private static bool Prepare(MethodBase? original)
    {
        return PatchTarget.IsPatchable(original);
    }

    [HarmonyTranspiler]
    [HarmonyPatch(nameof(NotificationBuff.OnRefresh), [])]
    private static IEnumerable<CodeInstruction> OnRefresh_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        // // 変更前
        // text = condition.GetText() + (EClass.debug.showExtra ? (" " + condition.value) : "");
        // // 変更後
        // text = condition.GetText() + (true ? (" " + condition.value) : "");
        var matcher = new CodeMatcher(instructions, generator);

        // ldfld bool CoreDebug::showExtra
        matcher.MatchEndForward(
            new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(CoreDebug), nameof(CoreDebug.showExtra)))
        );
        // バフ・デバフの値が表示されるようにする
        matcher.Advance(1);
        matcher.InsertAndAdvance(
            new CodeInstruction(OpCodes.Pop),
            new CodeInstruction(OpCodes.Ldc_I4_1)
        );

        return matcher.InstructionEnumeration();
    }
}
