using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text.RegularExpressions;
using HarmonyLib;
using ModUtility.Patch;

namespace DisplayDNAEnchantValues.Patches;

[HarmonyPatch(typeof(Element))]
public static class ElementPatch
{
    private static readonly ModPatchTarget PatchTarget = new();

    [HarmonyPrepare]
    private static bool Prepare(MethodBase? original)
    {
        return PatchTarget.IsPatchable(original);
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(Element.AddEncNote), [typeof(UINote), typeof(Card), typeof(ElementContainer.NoteMode), typeof(Func<Element, string, string>), typeof(Action<UINote, Element>)])]
    private static void AddEncNote_Prefix(Element __instance, UINote n, Card Card, ElementContainer.NoteMode mode, ref Func<Element, string, string>? funcText, Action<UINote, Element>? onAddNote)
    {
        if (funcText is null)
        {
            return;
        }

        // xキーの調査表示でエンチャント値が "(10) -> (10)" のように二重表示される可能性がある
        // 二重表示になっている場合、Modで追加したエンチャントの値を取り除く
        var textRegex = new Regex(@"(\(-?\d+\)) (\(-?\d+[^\)]*\))");
        var originalFuncText = funcText;
        funcText = (element, text) =>
        {
            return textRegex.Replace(originalFuncText(element, text), "$2");
        };
    }

    [HarmonyTranspiler]
    [HarmonyPatch(nameof(Element.AddEncNote), [typeof(UINote), typeof(Card), typeof(ElementContainer.NoteMode), typeof(Func<Element, string, string>), typeof(Action<UINote, Element>)])]
    private static IEnumerable<CodeInstruction> AddEncNote_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        // // 変更前
        // if (!flag && !flag2 && !source.tag.Contains("flag"))
        // {
        //     text = text + " [" + "*".Repeat(Mathf.Clamp(num * source.mtp / num4 + num3, 1, 5)) + ((num * source.mtp / num4 + num3 > 5) ? "+" : "") + "]";
        // }
        // // 変更後
        // if (!flag && !flag2 && !source.tag.Contains("flag"))
        // {
        //     text = text + " [" + "*".Repeat(Mathf.Clamp(num * source.mtp / num4 + num3, 1, 5)) + ((num * source.mtp / num4 + num3 > 5) ? "+" : "") + "]" + " (" + num.ToString() + ")";
        // }
        var matcher = new CodeMatcher(instructions, generator);

        // ldstr "flag"
        // call static bool ClassExtension::Contains(string[] strs, string id)
        // brtrue Label59
        matcher.MatchEndForward(
            new CodeMatch(OpCodes.Ldstr, "flag"),
            new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(ClassExtension), nameof(ClassExtension.Contains), [typeof(string[]), typeof(string)])),
            new CodeMatch(OpCodes.Brtrue)
        );
        // string::Concat(string[] value)の引数valueの要素数を5→8に変更
        // エンチャント値を追加するための枠を確保する
        matcher.Advance(1);
        matcher.RemoveInstruction();
        matcher.InsertAndAdvance(
            new CodeInstruction(OpCodes.Ldc_I4_8)
        );

        // ldstr "]"
        // stelem.ref NULL
        // call static string string::Concat(string[] values)
        matcher.MatchEndForward(
            new CodeMatch(OpCodes.Ldstr, "]"),
            new CodeMatch(OpCodes.Stelem_Ref),
            new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(string), nameof(string.Concat), [typeof(string[])]))
        );
        // string::Concat(string[] value)の引数valueにエンチャント値の要素を追加
        // value[5] = " ("
        // value[6] = num.ToString()
        // value[7] = ")"
        matcher.InsertAndAdvance(
            new CodeInstruction(OpCodes.Dup),
            new CodeInstruction(OpCodes.Ldc_I4_5),
            new CodeInstruction(OpCodes.Ldstr, " ("),
            new CodeInstruction(OpCodes.Stelem_Ref),
            new CodeInstruction(OpCodes.Dup),
            new CodeInstruction(OpCodes.Ldc_I4_6),
            new CodeInstruction(OpCodes.Ldloca_S, 5),
            CodeInstruction.Call(() => default(int).ToString()),
            new CodeInstruction(OpCodes.Stelem_Ref),
            new CodeInstruction(OpCodes.Dup),
            new CodeInstruction(OpCodes.Ldc_I4_7),
            new CodeInstruction(OpCodes.Ldstr, ")"),
            new CodeInstruction(OpCodes.Stelem_Ref)
        );

        return matcher.InstructionEnumeration();
    }
}
