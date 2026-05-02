using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using ModUtility.Patch;
using SomewhatEnhancedDisplay.Config;
using SomewhatEnhancedDisplay.Extensions;
using SomewhatEnhancedDisplay.UI;
using SomewhatEnhancedDisplay.UI.HoverGuide;
using UnityEngine;

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
    private static ModConfigHoverGuideStyleChara StyleConfig => Config.CurrentStyle.Chara;

    [HarmonyReversePatch(HarmonyReversePatchType.Original)]
    [HarmonyPatch(nameof(Chara.GetName), [typeof(NameStyle), typeof(int)])]
    private static string CharaGetNameForHoverText(Chara insrance, NameStyle nameStyle, int num = -1)
    {
        // Chara.GetName()をコードを複製し、ホバーテキスト取得処理向けに変更したスタブを作成する
        static IEnumerable<CodeInstruction> transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            // // 変更前
            // if (mimicry != null)
            // {
            // ...
            // if (HasCondition<ConTransmuteShadow>())
            // {
            // // 変更後
            // if (CharaPatch.IsMimicryEnabled() && mimicry != null)
            // {
            // ...
            // if (CharaPatch.IsShadowformEnabled() && HasCondition<ConTransmuteShadow>())
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

            // // ldstr "shade"
            matcher.MatchStartForward(
                new CodeMatch(OpCodes.Ldstr, "shade")
            );
            // Modの設定で影被りが無効になっている場合はシェイドの名前を取得しないようにする
            matcher.Advance(-4);
            var label2 = matcher.Operand;
            matcher.Advance(-2);
            matcher.InsertAndAdvance(
                CodeInstruction.Call(() => IsShadowformEnabled()),
                new CodeInstruction(OpCodes.Brfalse, label2)
            );

            return matcher.InstructionEnumeration();
        };

        _ = transpiler(null!, null!);
        return default!;
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
        // // 変更後
        // if (CharaPatch.IsMimicryEnabled() && mimicry != null && mimicry.IsThing)
        // {
        // ...
        // string text = ((mimicry != null && CharaPatch.IsMimicryEnabled()) ? mimicry.GetName(NameStyle.Full) : CharaPatch.CharaGetNameForHoverText(this, NameStyle.Full));
        // ...
        // return CharaPatch.BuildHoverText(text, text2, s, this);
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
        // Nameプロパティの呼び出しをCharaGetNameForHoverText(this, NameStyle.Full, -1)に置き換える
        matcher.Advance(1);
        matcher.RemoveInstruction();
        matcher.InsertAndAdvance(
            new CodeInstruction(OpCodes.Ldc_I4_1),
            new CodeInstruction(OpCodes.Ldc_I4_M1),
            CodeInstruction.Call(() => CharaGetNameForHoverText(default!, default, default))
        );
        // Modの設定で擬態が無効になっている場合は常に正体のキャラの名前を取得するようにする
        matcher.Advance(start - matcher.Pos);
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

        return matcher.InstructionEnumeration();
    }

    [HarmonyTranspiler]
    [HarmonyPatch(nameof(Chara.GetHoverText2), [])]
    private static IEnumerable<CodeInstruction> GetHoverText2_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        // // 変更前
        // if (mimicry != null && mimicry.IsThing)
        // {
        // ...
        // if (knowFav)
        // {
        // ...
        // text = text + "<size=14>" + "favgift".lang(GetFavCat().GetName().ToLower(), GetFavFood().GetName()) + "</size>";
        // ...
        // if (EClass.pc.held?.trait is TraitWhipLove && IsPCFaction)
        // {
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
        // if (CharaPatch.IsMimicryEnabled() && mimicry != null && mimicry.IsThing)
        // {
        // ...
        // if (true)
        // {
        // ...
        // text = text + $"<size=14>♡" + GetFavCat().GetName().ToLower() + "/" + GetFavFood().GetName() + "</size>";
        // ...
        // if (true || (EClass.pc.held?.trait is TraitWhipLove && IsPCFaction))
        // {
        // ...
        // text4 = CharaPatch.BuildStatsExtraText(text4, item3);
        // text3 = CharaPatch.ConcatStatsText(text3, text4.TagColor(c), ", ");
        // ...
        // else
        // {
        //      text3 = CharaPatch.BuildStatsText(text3);
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

        // ldarg.0 NULL
        // call bool Chara::get_knowFav()
        matcher.MatchStartForward(
            new CodeMatch(OpCodes.Ldarg_0),
            new CodeMatch(OpCodes.Call, AccessTools.PropertyGetter(typeof(Chara), nameof(Chara.knowFav)))
        );
        // 必ず好物を取得するようにする
        matcher.RemoveInstructions(2);
        matcher.InsertAndAdvance(
            new CodeInstruction(OpCodes.Ldc_I4_1)
        );

        // ldstr "<size=14>"
        // ldstr "favgift"
        matcher.Start();
        matcher.MatchStartForward(
            new CodeMatch(OpCodes.Ldstr, "<size=14>"),
            new CodeMatch(OpCodes.Ldstr, "favgift")
        );
        // "好物: "の文字列を"♡"に変更する
        matcher.Operand = matcher.Operand + "♡";
        matcher.Advance(1);
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

        // call static Chara EClass::get_pc() [Label4]
        // ldfld Card Chara::held
        // dup NULL
        // brtrue Label15
        // pop NULL
        // ldnull NULL
        // br Label16
        // ldfld Trait Card::trait [Label15]
        // isinst TraitWhipLove [Label16]
        // brfalse Label17
        // ldarg.0 NULL
        // callvirt virtual bool Card::get_IsPCFaction()
        // brfalse Label18
        // ldloc.1 NULL
        matcher.MatchStartForward(
            new CodeMatch(OpCodes.Call, AccessTools.PropertyGetter(typeof(EClass), nameof(EClass.pc))),
            new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(Chara), nameof(Chara.held))),
            new CodeMatch(OpCodes.Dup),
            new CodeMatch(OpCodes.Brtrue),
            new CodeMatch(OpCodes.Pop),
            new CodeMatch(OpCodes.Ldnull),
            new CodeMatch(OpCodes.Br),
            new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(Card), nameof(Card.trait))),
            new CodeMatch(OpCodes.Isinst, typeof(TraitWhipLove)),
            new CodeMatch(OpCodes.Brfalse),
            new CodeMatch(OpCodes.Ldarg_0),
            new CodeMatch(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(Card), nameof(Card.IsPCFaction))),
            new CodeMatch(OpCodes.Brfalse),
            new CodeMatch(OpCodes.Ldloc_1)
        );
        // 趣味・仕事を取得する処理への遷移先となるLabelMod1を生成する
        matcher.CreateLabelWithOffsets(13, out var labelMod1);
        // 必ず趣味・仕事を取得するようにする
        matcher.InsertAndAdvance(
            new CodeInstruction(OpCodes.Br, labelMod1)
        );
        // 前の条件分岐から追加した命令に遷移できるようにするため、ラベルを移動させる
        var labelList1 = matcher.Labels.Copy();
        matcher.Labels.Clear();
        matcher.Advance(-1);
        matcher.AddLabels(labelList1);

        // ldloc.2 NULL
        // ldstr "<size=14>"
        // call static string string::Concat(string str0, string str1)
        // stloc.2 NULL
        matcher.MatchStartForward(
            new CodeMatch(OpCodes.Ldloc_2),
            new CodeMatch(OpCodes.Ldstr, "<size=14>"),
            new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(string), nameof(string.Concat), [typeof(string), typeof(string)])),
            new CodeMatch(OpCodes.Stloc_2)
        );
        // sizeタグの開始タグを追加する処理を削除する
        matcher.RemoveInstructions(4);

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
            CodeInstruction.Call(() => BuildStatsExtraText(default!, default!)),
            new CodeInstruction(OpCodes.Stloc_S, 12)
        );

        // call static string ClassExtension::TagColor(string s, UnityEngine.Color c)
        // ldstr ", "
        // call static string string::Concat(string str0, string str1, string str2)
        matcher.MatchEndForward(
            new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(ClassExtension), nameof(ClassExtension.TagColor), [typeof(string), typeof(Color)])),
            new CodeMatch(OpCodes.Ldstr, ", "),
            new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(string), nameof(string.Concat), [typeof(string), typeof(string), typeof(string)]))
        );
        // バフ・デバフ・状態・呪いの文字列を行折り返しに対応した内容で結合する
        matcher.RemoveInstruction();
        matcher.InsertAndAdvance(
            new CodeInstruction(OpCodes.Ldloc_S, 9),
            CodeInstruction.Call(() => ConcatStatsText(default!, default!, default!, default))
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

        // ldstr ", "
        // call char[] string::ToCharArray()
        // callvirt string string::TrimEnd(char[] trimChars)
        // ldstr "</size>"
        // call static string string::Concat(string str0, string str1)
        // stloc.2 NULL
        // call static CoreDebug EClass::get_debug() [Label27, Label45]
        matcher.MatchStartForward(
            new CodeMatch(OpCodes.Ldstr, ", "),
            new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(string), nameof(string.ToCharArray), [])),
            new CodeMatch(OpCodes.Callvirt, AccessTools.Method(typeof(string), nameof(string.TrimEnd), [typeof(char[])])),
            new CodeMatch(OpCodes.Ldstr, "</size>"),
            new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(string), nameof(string.Concat), [typeof(string), typeof(string)])),
            new CodeMatch(OpCodes.Stloc_2),
            new CodeMatch(OpCodes.Call, AccessTools.PropertyGetter(typeof(EClass), nameof(EClass.debug)))
        );
        // 末尾の要素から不要な文字列を削除する処理を変更する
        matcher.RemoveInstructions(5);
        matcher.InsertAndAdvance(
            CodeInstruction.Call(() => BuildStatsText(default!))
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

    private static bool IsShadowformEnabled()
    {
        return StyleConfig.EnableShadowform;
    }

    private static bool IsMimicryEnabled()
    {
        return StyleConfig.EnableMimicry;
    }

    private static string IntToString(int value)
    {
        return value.ToString();
    }

    private static int ComputeFontSize(int size)
    {
        // フォントサイズを微調整する
        return ModUIUtil.ComputeFontSize(size - 1);
    }

    public static string BuildStatsExtraText(string text4, BaseStats stats)
    {
        if (!StyleConfig.DisplayStatsValue)
        {
            return text4;
        }
        var statsValueText = $"({stats.GetValue()})".TagSize(12);
        return $"{text4}{statsValueText}";
    }

    public static string ConcatStatsText(string text3, string text4, string separator, int num)
    {
        var config = StyleConfig.StatsLineWrapping;
        var newline = string.Empty;
        if (config.Enable && config.MaxItemsPerLine > 0 && num >= config.MaxItemsPerLine && num % config.MaxItemsPerLine == 0)
        {
            newline = Environment.NewLine;
            separator = string.Empty;
        }
        return $"{text3}{$"{text4}{separator}".TagSize(14)}{newline}";
    }

    public static string BuildStatsText(string text3)
    {
        var textEnd = ", </size>";
        if (text3.EndsWith(Environment.NewLine))
        {
            text3 = text3.Substring(0, text3.Length - Environment.NewLine.Length);
            textEnd = "</size>";
        }
        return $"{text3.Substring(0, text3.LastIndexOf(textEnd))}</size>";
    }

    private static string BuildHoverText(string text, string text2, string s, Chara chara)
    {
        text = text.TagResize(ComputeFontSize);
        text2 = text2.TagResize(ComputeFontSize);
        s = s.TagResize(ComputeFontSize);
        return ModCharaHoverTextBuilder.BuildHoverText(chara, text, text2, s);
    }

    private static string BuildHoverText2(string text, string text2, string text3, Chara chara)
    {
        text = text.TagResize(ComputeFontSize);
        text2 = text2.TagResize(ComputeFontSize);
        text3 = text3.TagResize(ComputeFontSize);
        return ModCharaHoverTextBuilder.BuildHoverText2(chara, text, text2, text3);
    }
}
