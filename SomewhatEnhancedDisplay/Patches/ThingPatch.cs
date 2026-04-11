using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using ModUtility.Patch;
using SomewhatEnhancedDisplay.Extensions;
using SomewhatEnhancedDisplay.UI;

namespace SomewhatEnhancedDisplay.Patches;

[HarmonyPatch(typeof(Thing))]
public static class ThingPatch
{
    private static readonly ModPatchTarget PatchTarget = new();

    [HarmonyPrepare]
    private static bool Prepare(MethodBase? original)
    {
        return PatchTarget.IsPatchable(original);
    }

    [HarmonyTranspiler]
    [HarmonyPatch(nameof(Thing.GetHoverText), [])]
    private static IEnumerable<CodeInstruction> GetHoverText_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        // // 変更前
        // string text = "";
        // ...
        // " <size=14>(""
        // ...
        // return base.GetHoverText() + text;
        // // 変更後
        // string text = "";
        // string? localTraitText;
        // ...
        // $" <size={CharaPatch.ComputeFontSize(14)}>("
        // ...
        // return ModThingHoverTextBuilder.BuildHoverText(base.GetHoverText(), text, localTraitText, this);
        var matcher = new CodeMatcher(instructions, generator);

        // ldstr " <size=14>(""
        matcher.MatchStartForward(
            new CodeMatch(OpCodes.Ldstr, " <size=14>(")
        );
        // フォントサイズのタグを構築する処理をすべて差し替える
        matcher.RemoveInstruction();
        matcher.InsertAndAdvance(
            new CodeInstruction(OpCodes.Ldstr, " <size="),
            new CodeInstruction(OpCodes.Ldc_I4_S, 14),
            CodeInstruction.Call(() => ComputeFontSize(default)),
            CodeInstruction.Call(() => IntToString(default)),
            new CodeInstruction(OpCodes.Ldstr, ">("),
            CodeInstruction.Call(() => string.Concat(default, default, default))
        );

        // brtrue Label6
        // ldloc.0 NULL
        // call static string Environment::get_NewLine()
        matcher.MatchStartForward(
            new CodeMatch(OpCodes.Brtrue),
            new CodeMatch(OpCodes.Ldloc_0),
            new CodeMatch(OpCodes.Call, AccessTools.PropertyGetter(typeof(Environment), nameof(Environment.NewLine)))
        );
        // train.GetHoverText()の戻り値がtextに追加されないようにする
        matcher.Advance(1);
        matcher.RemoveInstructions(5);

        // call static string string::Concat(string str0, string str1)
        // ret NULL
        matcher.MatchStartForward(
            new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(string), nameof(string.Concat), [typeof(string), typeof(string)])),
            new CodeMatch(OpCodes.Ret)
        );
        // 表示内容の文字列を組み立てる処理を差し替える
        matcher.RemoveInstruction();
        matcher.InsertAndAdvance(
            new CodeInstruction(OpCodes.Ldarg_0),
             CodeInstruction.Call(() => BuildHoverText(default!, default!, default!))
        );

        return matcher.InstructionEnumeration();
    }

    private static int ComputeFontSize(int fontSize)
    {
        // ゲーム設定のウィジェットのフォントサイズが "大きめ" の場合を基準にする
        return ModUIUtil.ComputeFontSize(fontSize - 3);
    }

    private static string IntToString(int value)
    {
        return value.ToString();
    }

    private static string BuildHoverText(string cardText, string text, Thing thing)
    {
        return ModThingHoverTextBuilder.BuildHoverText(thing, cardText, text);
    }
}
