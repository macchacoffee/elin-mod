using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using ModUtility.Patch;

namespace SomewhatEnhancedDisplay.Patches;

[HarmonyPatch(typeof(NotificationStats))]
public static class NotificationStatsPatch
{
    private static readonly ModPatchTarget PatchTarget = new();

    [HarmonyPrepare]
    private static bool Prepare(MethodBase? original)
    {
        return PatchTarget.IsPatchable(original);
    }

    [HarmonyTranspiler]
    [HarmonyPatch(nameof(NotificationStats.OnRefresh), [])]
    private static IEnumerable<CodeInstruction> OnRefresh_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        // // 変更前
        // text = baseStats.GetText() + ((EClass.debug.showExtra && !baseStats.GetText().IsEmpty()) ? ("(" + baseStats.GetValue() + ")") : "");
        // // 変更後
        // text = baseStats.GetText() + ((true && !baseStats.GetText().IsEmpty()) ? ("(" + baseStats.GetValue() + ")") : "");
        var matcher = new CodeMatcher(instructions, generator);

        // ldfld bool CoreDebug::showExtra
        matcher.MatchEndForward(
            new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(CoreDebug), nameof(CoreDebug.showExtra)))
        );
        // 状態の値が表示されるようにする
        matcher.Advance(1);
        matcher.InsertAndAdvance(
            new CodeInstruction(OpCodes.Pop),
            new CodeInstruction(OpCodes.Ldc_I4_1)
        );

        return matcher.InstructionEnumeration();
    }
}
