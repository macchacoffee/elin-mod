using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using ModUtility.Patch;
using SomewhatEnhancedDisplay.UI;

namespace SomewhatEnhancedDisplay.Patches;

[HarmonyPatch(typeof(Chara))]
public static class CharaPatch
{
    private static readonly ModPatchTarget PatchTarget = new();

    [HarmonyPrepare]
    private static bool Prepare(MethodBase? original)
    {
        return PatchTarget.IsPatchable(original);
    }

    [HarmonyTranspiler]
    [HarmonyPatch(nameof(Chara.GetHoverText), [])]
    private static IEnumerable<CodeInstruction> GetHoverText_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        // // 変更前
        // return text + text2 + s;
        // ...
        // .TagSize(...)
        // // 変更後
        // return ModHoverTextBuilder.BuildHoverText(text, text2, s, this);
        // ...
        // .TagSize(CharaPatch.ComputeFontSize(...))
        var matcher = new CodeMatcher(instructions, generator);

        // call static string string::Concat(string str0, string str1, string str2)
        // ret NULL
        matcher.MatchStartForward(
            new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(string), nameof(string.Concat), [typeof(string), typeof(string), typeof(string)])),
            new CodeMatch(OpCodes.Ret)
        );
        // 表示内容の文字列を組み立てる処理を差し替える
        matcher.RemoveInstruction();
        matcher.InsertAndAdvance(
            new CodeInstruction(OpCodes.Ldarg_0),
            CodeInstruction.Call(() => ModHoverTextBuilder.BuildHoverText(default!, default!, default!, default!))
        );

        // call static string ClassExtension::TagColor(string s, UnityEngine.Color c)
        matcher.Start();
        matcher.MatchStartForward(
            new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(ClassExtension), nameof(ClassExtension.TagSize), [typeof(string), typeof(int)]))
        );
        // フォントサイズのタグを生成する処理をすべて差し替える
        matcher.Repeat(m =>
        {
            m.InsertAndAdvance(
                CodeInstruction.Call(() => ComputeFontSize(default))
            );
            m.Advance(1);
        });

        return matcher.InstructionEnumeration();
    }

    [HarmonyTranspiler]
    [HarmonyPatch(nameof(Chara.GetHoverText2), [])]
    private static IEnumerable<CodeInstruction> GetHoverText2_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        // // 変更前
        // "<size=14>"
        // ...
        // text = text + "<size=14>" + "favgift".lang(GetFavCat().GetName().ToLower(), GetFavFood().GetName()) + "</size>";
        // ...
        // text3 = text3 + text4.TagColor(c) + ", ";
        // ...
        // else
        // {
        //      text = "";
        //      text3 = text3.TrimEnd(", ".ToCharArray()) + "</size>";
        // }
        // ...
        // return text + text2 + text3;
        // // 変更後
        // $"<size={CharaPatch.ComputeFontSize(14)}>"
        // ...
        // text = text + $"<size={CharaPatch.ComputeFontSize(14)}>♡" + GetFavCat().GetName().ToLower() + "/" + GetFavFood().GetName() + "</size>";
        // ...
        // text4 = ModHoverTextBuilder.BuildStatsExtraText(text4, item3);
        // text3 = text3 + text4.TagColor(c) + ", ";
        // ...
        // else
        // {
        //      text3 = text3.TrimEnd(", ".ToCharArray()) + "</size>";
        // }
        // ...
        // return ModHoverTextBuilder.BuildHoverText2(text, text2, text3, this);
        var matcher = new CodeMatcher(instructions, generator);

        // ldstr "<size=14>"
        matcher.MatchStartForward(
            new CodeMatch(OpCodes.Ldstr, "<size=14>")
        );
        // フォントサイズのタグを構築する処理をすべて差し替える
        matcher.Repeat(m =>
        {
            m.RemoveInstruction();
            m.InsertAndAdvance(
                new CodeInstruction(OpCodes.Ldstr, "<size="),
                new CodeInstruction(OpCodes.Ldc_I4_S, 14),
                CodeInstruction.Call(() => ComputeFontSize(default)),
                CodeInstruction.Call(() => IntToString(default)),
                new CodeInstruction(OpCodes.Ldstr, ">"),
                CodeInstruction.Call(() => string.Concat(default, default, default))
            );
        });

        // ldstr ">"
        // call static string string::Concat(string str0, string str1, string str2)
        // ldstr "favgift"
        matcher.Start();
        matcher.MatchStartForward(
            new CodeMatch(OpCodes.Ldstr, ">"),
            new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(string), nameof(string.Concat), [typeof(string), typeof(string), typeof(string)])),
            new CodeMatch(OpCodes.Ldstr, "favgift")
        );
        // "好物: "の文字列を"♡"に変更する
        matcher.Operand = matcher.Operand + "♡";
        matcher.Advance(2);
        matcher.RemoveInstruction();

        // callvirt string string::ToLower()
        // ldarg.0 NULL
        // call SourceThing+Row Chara::GetFavFood()
        matcher.MatchStartForward(
            new CodeMatch(OpCodes.Callvirt, AccessTools.Method(typeof(string), nameof(string.ToLower), [])),
            new CodeMatch(OpCodes.Ldarg_0),
            new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(Chara), nameof(Chara.GetFavFood), []))
        );
        // 好物のカテゴリーと好きな食べ物の間に"/"を挿入する
        matcher.Advance(1);
        matcher.InsertAndAdvance(
            new CodeInstruction(OpCodes.Ldstr, "/")
        );

        // callvirt virtual string SourceData+BaseRow::GetName()
        // ldnull NULL
        // ldnull NULL
        // ldnull NULL
        // call static string ClassExtension::lang(string s, string ref1, string ref2, string ref3, string ref4, string ref5)
        matcher.MatchStartForward(
            new CodeMatch(OpCodes.Callvirt, AccessTools.Method(typeof(SourceData.BaseRow), nameof(SourceData.BaseRow.GetName), [])),
            new CodeMatch(OpCodes.Ldnull),
            new CodeMatch(OpCodes.Ldnull),
            new CodeMatch(OpCodes.Ldnull),
            new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(ClassExtension), nameof(ClassExtension.lang), [typeof(string), typeof(string), typeof(string), typeof(string), typeof(string), typeof(string)]))
        );
        // 好物のカテゴリー名、"/"、好きな食べ物名の文字列を結合する
        matcher.Advance(1);
        matcher.RemoveInstructions(4);
        matcher.InsertAndAdvance(
            new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(string), nameof(string.Concat), [typeof(string), typeof(string), typeof(string)]))
        );

        // add NULL
        // stloc.s 9 (System.Int32)
        matcher.MatchEndForward(
            new CodeMatch(OpCodes.Add),
            new CodeMatch(OpCodes.Stloc_S)
        );
        // バフ・デバフ・状態・呪いの文字列にパワーを追加する
        matcher.Advance(1);
        matcher.InsertAndAdvance(
            new CodeInstruction(OpCodes.Ldloc_S, 12),
            new CodeInstruction(OpCodes.Ldloc_S, 11),
            CodeInstruction.Call(() => ModHoverTextBuilder.BuildStatsExtraText(default!, default!)),
            new CodeInstruction(OpCodes.Stloc_S, 12)
        );

        // br Label43
        // ldstr "" [Label42]
        // stloc.0 NULL
        matcher.MatchEndForward(
            new CodeMatch(OpCodes.Br),
            new CodeMatch(OpCodes.Ldstr, ""),
            new CodeMatch(OpCodes.Stloc_0)
        );
        // バフ・デバフ・状態・呪いの文字列が存在する時に好物の文字列がクリアされないようにする
        matcher.RemoveInstruction();
        matcher.InsertAndAdvance(
            new CodeInstruction(OpCodes.Pop)
        );

        // call static string string::Concat(string str0, string str1, string str2)
        // ret NULL
        matcher.MatchStartForward(
            new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(string), nameof(string.Concat), [typeof(string), typeof(string), typeof(string)])),
            new CodeMatch(OpCodes.Ret)
        );
        // 表示内容の文字列を組み立てる処理を差し替える
        matcher.RemoveInstruction();
        matcher.InsertAndAdvance(
            new CodeInstruction(OpCodes.Ldarg_0),
             CodeInstruction.Call(() => ModHoverTextBuilder.BuildHoverText2(default!, default!, default!, default!))
        );

        return matcher.InstructionEnumeration();
    }

    private static int ComputeFontSize(int fontSize)
    {
        return ModUIUtil.ComputeFontSize(fontSize - 3);
    }

    private static string IntToString(int value)
    {
        return value.ToString();
    }
}
