using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using ModUtility.Patch;

namespace SomewhatEnhancedDisplay.Patches;

[HarmonyPatch(typeof(WidgetMouseover))]
public static class WidgetMouseoverPatch
{
    private static readonly ModPatchTarget PatchTarget = new();

    [HarmonyPrepare]
    private static bool Prepare(MethodBase? original)
    {
        return PatchTarget.IsPatchable(original);
    }

    [HarmonyTranspiler]
    [HarmonyPatch(nameof(WidgetMouseover.Refresh), [])]
    private static IEnumerable<CodeInstruction> Refresh_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        // // 変更前
        // text += "otherCards".lang((count - 1).ToString() ?? "");
        // // 変更後
        // text = WidgetMouseoverPatch.BuildGHoverText(text, "otherCards".lang((count - 1).ToString() ?? ""));
        var matcher = new CodeMatcher(instructions, generator);

        // call static string ClassExtension::lang(string s, string ref1, string ref2, string ref3, string ref4, string ref5)
        // call static string string::Concat(string str0, string str1)
        matcher.MatchEndForward(
            new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(ClassExtension), nameof(ClassExtension.lang), [typeof(string), typeof(string), typeof(string), typeof(string), typeof(string), typeof(string)])),
            new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(string), nameof(string.Concat), [typeof(string), typeof(string)]))
        );
        // "(他+n)"の文字列を調整し、ホバーテキストの1行目末尾に追加されるようにする
        matcher.RemoveInstruction();
        matcher.InsertAndAdvance(
             CodeInstruction.Call(() => BuildHoverText(default!, default!))
        );

        return matcher.InstructionEnumeration();
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(WidgetMouseover.Show), [typeof(string)])]
    private static void Show_Prefix(WidgetMouseover __instance, string s)
    {
        // 15行表示の場合にpivotのyが0.8fとなるようにする
        var lineCount = s.SplitByNewline().Length;
        __instance.layout.Rect().pivot =  new(0.5f, 0.5f * (1 + 0.043f * (lineCount - 1)));
    }

    private static string BuildHoverText(string hoverText, string otherCardsText)
    {
        var lines = hoverText.SplitByNewline();
        lines[0] = $"{lines[0]}{otherCardsText.TagSize(14)}";
        return string.Join(Environment.NewLine, lines);
    }
}
