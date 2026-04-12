using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using ModUtility.Patch;
using SomewhatEnhancedDisplay.Config;
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

    private static ModConfigHoverGuide Config => Mod.Config.HoverGuide;
    private static ModConfigHoverGuideProfileChara ProfileConfig => Config.CurrentProfile.Chara;

    [HarmonyTranspiler]
    [HarmonyPatch(nameof(Chara.GetName), [typeof(NameStyle), typeof(int)])]
    private static IEnumerable<CodeInstruction> GetName_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        // // 変更前
        // if (mimicry != null)
        // {
        // // 変更後
        // if (CharaPatch.IsMimicryEnabled() && mimicry != null)
        // {
        var matcher = new CodeMatcher(instructions, generator);

        // ldfld ConBaseTransmuteMimic Chara::mimicry
        // brfalse Label1
        matcher.MatchEndForward(
            new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(Chara), nameof(Chara.mimicry))),
            new CodeMatch(OpCodes.Brfalse)
        );
        // Modの設定で擬態が無効になっている場合は擬態先の名前を取得しないようにする
        var label1 = matcher.Operand;
        matcher.Advance(-2);
        matcher.InsertAndAdvance(
            CodeInstruction.Call(() => IsMimicryEnabled()),
            new CodeInstruction(OpCodes.Brfalse, label1)
        );

        return matcher.InstructionEnumeration();
    }

    [HarmonyTranspiler]
    [HarmonyPatch(nameof(Chara.GetHoverText), [])]
    private static IEnumerable<CodeInstruction> GetHoverText_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        // // 変更前
        // if (mimicry != null && mimicry.IsThing)
        // {
        // ...
        // string text = ((mimicry != null) ? mimicry.GetName(NameStyle.Full) : base.Name);
        // ...
        // return text + text2 + s;
        // ...
        // .TagSize(...)
        // // 変更後
        // if (CharaPatch.IsMimicryEnabled() && mimicry != null && mimicry.IsThing)
        // {
        // ...
        // string text = ((mimicry != null && CharaPatch.IsMimicryEnabled()) ? mimicry.GetName(NameStyle.Full) : base.Name);
        // ...
        // return BuildHoverText(text, text2, s, this);
        // ...
        // .TagSize(CharaPatch.ComputeFontSize(...))
        var matcher = new CodeMatcher(instructions, generator);

        // ldfld ConBaseTransmuteMimic Chara::mimicry
        // brfalse Label1
        matcher.MatchEndForward(
            new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(Chara), nameof(Chara.mimicry))),
            new CodeMatch(OpCodes.Brfalse)
        );
        // Modの設定で擬態が無効になっている場合は擬態先のホバーテキストを取得しないようにする
        var label1 = matcher.Operand;
        matcher.Advance(-2);
        matcher.InsertAndAdvance(
            CodeInstruction.Call(() => IsMimicryEnabled()),
            new CodeInstruction(OpCodes.Brfalse, label1)
        );

        // ret NULL
        // ldarg.0 NULL [Label1, Label2]
        // ldfld ConBaseTransmuteMimic Chara::mimicry
        // brtrue Label3
        matcher.MatchEndForward(
            new CodeMatch(OpCodes.Ret),
            new CodeMatch(OpCodes.Ldarg_0),
            new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(Chara), nameof(Chara.mimicry))),
            new CodeMatch(OpCodes.Brtrue)
        );
        // 後で生成するラベルへ遷移する処理を挿入する場所を保存する
        var start = matcher.Pos;

        // ldarg.0 NULL
        // call string Card::get_Name()
        matcher.MatchStartForward(
            new CodeMatch(OpCodes.Ldarg_0),
            new CodeMatch(OpCodes.Call, AccessTools.PropertyGetter(typeof(Card), nameof(Card.Name)))
        );
        // Modの設定で擬態が無効になっている場合の遷移先となるLabelMod1を生成する
        matcher.CreateLabel(out var LabelMod1);
        matcher.Advance(start - matcher.Pos);
        // Modの設定で擬態が無効になっている場合は常に正体のキャラの名前を取得するようにする
        matcher.InsertAndAdvance(
            new CodeInstruction(OpCodes.Brfalse, LabelMod1),
            CodeInstruction.Call(() => IsMimicryEnabled())
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
            CodeInstruction.Call(() => BuildHoverText(default!, default!, default!, default!))
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
        // text4 = ModCharaHoverTextBuilder.BuildStatsExtraText(text4, item3);
        // text3 = text3 + text4.TagColor(c) + ", ";
        // ...
        // else
        // {
        //      text3 = text3.TrimEnd(", ".ToCharArray()) + "</size>";
        // }
        // ...
        // return BuildHoverText2(text, text2, text3, this);
        var matcher = new CodeMatcher(instructions, generator);

        // ldfld ConBaseTransmuteMimic Chara::mimicry
        // brfalse Label1
        matcher.MatchEndForward(
            new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(Chara), nameof(Chara.mimicry))),
            new CodeMatch(OpCodes.Brfalse)
        );
        // Modの設定で擬態が無効になっている場合は擬態先のホバーテキストを取得しないようにする
        var label1 = matcher.Operand;
        matcher.Advance(-2);
        matcher.InsertAndAdvance(
            CodeInstruction.Call(() => IsMimicryEnabled()),
            new CodeInstruction(OpCodes.Brfalse, label1)
        );

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
            CodeInstruction.Call(() => ModCharaHoverTextBuilder.BuildStatsExtraText(default!, default!)),
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
             CodeInstruction.Call(() => BuildHoverText2(default!, default!, default!, default!))
        );

        return matcher.InstructionEnumeration();
    }

    private static bool IsMimicryEnabled()
    {
        Plugin.LogInfo($"IsMimicryEnabled: {ProfileConfig.EnableMimicry}");
        return ProfileConfig.EnableMimicry;
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

    private static string BuildHoverText(string text, string text2, string s, Chara chara)
    {
        return ModCharaHoverTextBuilder.BuildHoverText(chara, text, text2, s);
    }

    private static string BuildHoverText2(string text, string text2, string text3, Chara chara)
    {
        return ModCharaHoverTextBuilder.BuildHoverText2(chara, text, text2, text3);
    }
}
