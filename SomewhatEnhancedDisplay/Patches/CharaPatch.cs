using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;
using HarmonyLib;
using ModUtility.Patch;
using SomewhatEnhancedDisplay.Config;
using SomewhatEnhancedDisplay.Extensions;
using SomewhatEnhancedDisplay.UI;
using SomewhatEnhancedDisplay.UI.HoverGuide;

namespace SomewhatEnhancedDisplay.Patches;

[HarmonyPatch(typeof(Chara))]
public static class CharaPatch
{
    private static readonly ModPatchTarget _patchTarget = new();

    [HarmonyPrepare]
    private static bool Prepare(MethodBase? original)
    {
        return _patchTarget.IsPatchable(original) && ModContext.Config.EnableHoverGuide.Value;
    }

    private static ModConfigHoverGuide Config => ModContext.WorldConfig.HoverGuide;
    private static ModConfigHoverGuideStyleChara StyleConfig => Config.CurrentStyle.Chara;

    [HarmonyReversePatch(HarmonyReversePatchType.Original)]
    [HarmonyPatch(nameof(Chara.GetName), [typeof(NameStyle), typeof(int)])]
    private static string CharaGetNameForHoverText(Chara instance, NameStyle nameStyle, int num = -1)
    {
        // Chara.GetName()のコードを複製し、ホバーテキスト取得処理向けに変更したスタブを作成する
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
        }

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
        // string text = ((mimicry != null && mimicry.Card != this) ? mimicry.GetName(NameStyle.Full) : base.Name);
        // ...
        // string text2 = Lang.GetList("lvComparison")[num];
        // text2 = (" (" + text2 + ") ").TagSize(14).TagColor(EClass.Colors.gradientLVComparison.Evaluate(0.25f * (float)num));
        // ...
        // if (memberType == FactionMemberType.Guest)
        // {
        //     s += (" (" + "guest".lang() + ") ").TagSize(14);
        // }
        // else if (memberType == FactionMemberType.Livestock)
        // {
        //     s += (" (" + "livestock".lang() + ") ").TagSize(14);
        // }
        // ...
        // if (!EClass.pc.IsMoving)
        // {
        // ...
        // if (Evalue(1232) > 0)
        // {
        // ...
        // if (Guild.Fighter.ShowBounty(this) && Guild.Fighter.HasBounty(this))
        // {
        // ...
        // if (EClass.pc.HasElement(481))
        // {
        // ...
        // if (EClass.pc.HasElement(6607))
        // {
        //     s += CraftUtil.GetBloodText(this).TagSize(14).TagColor(EClass.Colors.colorBlood);
        // }
        // ...
        // return text + text2 + s;
        // // 変更後
        // if (CharaPatch.IsMimicryEnabled() && mimicry != null && mimicry.IsThing)
        // {
        // ...
        // if (CharaPatch.DisplaysLvComparison())
        // {
        //      string text2 = Lang.GetList("lvComparison")[num];
        //      text2 = ("(" + text2 + ") ").TagSize(14).TagColor(EClass.Colors.gradientLVComparison.Evaluate(0.25f * (float)num));
        // }
        // else
        // {
        //      text2 = "";
        // }
        // ...
        // string text = ((mimicry != null && mimicry.Card != this && CharaPatch.IsMimicryEnabled()) ? mimicry.GetName(NameStyle.Full) : CharaPatch.CharaGetNameForHoverText(this, NameStyle.Full));
        // // ...
        // if (CharaPatch.DisplaysFactionMemberType())
        // {
        //     if (memberType == FactionMemberType.Guest)
        //     {
        //         s += (" (" + "guest".lang() + ") ").TagSize(14);
        //     }
        //     else if (memberType == FactionMemberType.Livestock)
        //     {
        //         s += (" (" + "livestock".lang() + ") ").TagSize(14);
        //     }
        // }
        // ...
        // if (!EClass.pc.IsMoving && CharaPatch.DisplaysHeightDifference())
        // {
        // ...
        // if (Evalue(1232) > 0 && CharaPatch.DisplaysMilkBaby())
        // {
        // ...
        // if (Guild.Fighter.ShowBounty(this) && Guild.Fighter.HasBounty(this) && CharaPatch.DisplaysBounty())
        // {
        // ...
        // if ((EClass.pc.HasElement(481) && CharaPatch.DisplaysFaith()) || CharaPatch.DisplaysAlwaysFaith())
        // {
        // ...
        // if (EClass.pc.HasElement(6607) && CharaPatch.DisplaysBloodTaste()) || CharaPatch.DisplaysAlwaysBloodTaste())
        // {
        //     s += CraftUtilPatch.CraftUtilGetBloodTextForCharaHoverText(this).TagSize(14).TagColor(EClass.Colors.colorBlood);
        // }
        // ...
        // string text = ((mimicry != null && mimicry.Card != this && CharaPatch.IsMimicryEnabled()) ? mimicry.GetName(NameStyle.Full) : CharaPatch.CharaGetNameForHoverText(this, NameStyle.Full));
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

        // ldarg.0 NULL
        // ldfld ConBaseTransmuteMimic Chara::mimicry
        // callvirt virtual Card ConBaseTransmuteMimic::get_Card()
        // ldarg.0 NULL
        // bne.un Label4
        matcher.MatchEndForward(
            new CodeMatch(OpCodes.Ldarg_0),
            new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(Chara), nameof(Chara.mimicry))),
            new CodeMatch(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(ConBaseTransmuteMimic), nameof(ConBaseTransmuteMimic.Card))),
            new CodeMatch(OpCodes.Ldarg_0),
            new CodeMatch(OpCodes.Bne_Un)
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
            new CodeInstruction(OpCodes.Beq, LabelMod1),
            CodeInstruction.Call(() => IsMimicryEnabled())
        );
        matcher.Opcode = OpCodes.Brtrue;

        // ldfld UnityEngine.Gradient ColorProfile::gradientLVComparison
        matcher.MatchStartForward(
            new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(ColorProfile), nameof(ColorProfile.gradientLVComparison)))
        );
        // callvirt UnityEngine.Color UnityEngine.Gradient::Evaluate(float time)
        // call static string ClassExtension::TagColor(string s, UnityEngine.Color c)
        // stloc.3 NULL
        matcher.MatchEndForward(
            new CodeMatch(OpCodes.Callvirt, AccessTools.Method(typeof(Gradient), nameof(Gradient.Evaluate), [typeof(float)])),
            new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(ClassExtension), nameof(ClassExtension.TagColor), [typeof(string), typeof(Color)])),
            new CodeMatch(OpCodes.Stloc_3)
        );
        // レベル差の文字列を追加する条件でModの設定を参照するように変更する
        matcher.CreateLabel(out var labelModLvComparison1);
        matcher.InsertAndAdvance(
            new CodeInstruction(OpCodes.Br, labelModLvComparison1),
            new CodeInstruction(OpCodes.Ldstr, "")
        );
        matcher.CreateLabelWithOffsets(-1, out var labelModLvComparison2);
        // ldstr "lvComparison" [Label10, Label12, Label14, Label15]
        matcher.MatchStartBackwards(
            new CodeMatch(OpCodes.Ldstr, "lvComparison")
        );
        var labelListLvComparison1 = matcher.Labels.Copy();
        matcher.Labels.Clear();
        matcher.Insert(
            CodeInstruction.Call(() => DisplaysLvComparison()),
            new CodeInstruction(OpCodes.Brfalse, labelModLvComparison2)
        );
        matcher.AddLabels(labelListLvComparison1);

        // bne.un Label22
        // ldloc.s 4 (System.String)
        // ldstr " ("
        // ldstr "livestock"
        matcher.MatchStartForward(
            new CodeMatch(OpCodes.Bne_Un),
            new CodeMatch(OpCodes.Ldloc_S),
            new CodeMatch(OpCodes.Ldstr, " ("),
            new CodeMatch(OpCodes.Ldstr, "livestock")
        );
        // 拠点のメンバータイプの文字列を追加する条件でModの設定を参照するように変更する
         var labelFactionMemberType1 = matcher.Operand;
        // ldarg.0 NULL
        // ldfld FactionMemberType Chara::memberType
        // ldc.i4.4 NULL
        // bne.un Label20
        // ldloc.s 4 (System.String)
        // ldstr " ("
        // ldstr "guest"
        matcher.MatchStartBackwards(
            new CodeMatch(OpCodes.Ldarg_0),
            new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(Chara), nameof(Chara.memberType))),
            new CodeMatch(OpCodes.Ldc_I4_4),
            new CodeMatch(OpCodes.Bne_Un),
            new CodeMatch(OpCodes.Ldloc_S),
            new CodeMatch(OpCodes.Ldstr, " ("),
            new CodeMatch(OpCodes.Ldstr, "guest")
        );
        matcher.InsertAndAdvance(
            CodeInstruction.Call(() => DisplaysFactionMemberType()),
            new CodeInstruction(OpCodes.Brfalse, labelFactionMemberType1)
        );

        // call static Chara EClass::get_pc() [Label21, Label22]
        // callvirt virtual bool Card::get_IsMoving()
        matcher.MatchEndForward(
            new CodeMatch(OpCodes.Call, AccessTools.PropertyGetter(typeof(EClass), nameof(EClass.pc))),
            new CodeMatch(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(Card), nameof(Card.IsMoving)))
        );
        // 高低差の文字列を追加する条件でModの設定を参照するように変更する
        matcher.Advance(1);
        var labelHeightDifference1 = matcher.Operand;
        matcher.Advance(1);
        matcher.InsertAndAdvance(
            CodeInstruction.Call(() => DisplaysHeightDifference()),
            new CodeInstruction(OpCodes.Brfalse, labelHeightDifference1)
        );

        // ldarg.0 NULL [Label23, Label25, Label26]
        // ldc.i4 1232
        // call int Card::Evalue(int ele)
        // ldc.i4.0 NULL
        matcher.MatchEndForward(
            new CodeMatch(OpCodes.Ldarg_0),
            new CodeMatch(OpCodes.Ldc_I4, 1232),
            new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(Card), nameof(Card.Evalue), [typeof(int)])),
            new CodeMatch(OpCodes.Ldc_I4_0)
        );
        // 赤ちゃんの文字列を追加する条件でModの設定を参照するように変更する
        matcher.Advance(1);
        var labelMilkBaby1 = matcher.Operand;
        matcher.Advance(1);
        matcher.InsertAndAdvance(
            CodeInstruction.Call(() => DisplaysMilkBaby()),
            new CodeInstruction(OpCodes.Brfalse, labelMilkBaby1)
        );

        // ldarg.0 NULL
        // callvirt bool GuildFighter::HasBounty(Chara c)
        matcher.MatchEndForward(
            new CodeMatch(OpCodes.Ldarg_0),
            new CodeMatch(OpCodes.Callvirt, AccessTools.Method(typeof(GuildFighter), nameof(GuildFighter.HasBounty), [typeof(Chara)]))
        );
        // 賞金首の文字列を追加する条件でModの設定を参照するように変更する
        matcher.Advance(1);
        var labelBounty1 = matcher.Operand;
        matcher.Advance(1);
        matcher.InsertAndAdvance(
            CodeInstruction.Call(() => DisplaysBounty()),
            new CodeInstruction(OpCodes.Brfalse, labelBounty1)
        );

        // call static Chara EClass::get_pc() [Label28, Label29]
        // ldc.i4 481
        // ldc.i4.0 NULL
        // callvirt bool Card::HasElement(int ele, bool includeNagative)
        matcher.MatchEndForward(
            new CodeMatch(OpCodes.Call, AccessTools.PropertyGetter(typeof(EClass), nameof(EClass.pc))),
            new CodeMatch(OpCodes.Ldc_I4, 481),
            new CodeMatch(OpCodes.Ldc_I4_0),
            new CodeMatch(OpCodes.Callvirt, AccessTools.Method(typeof(Card), nameof(Card.HasElement), [typeof(int), typeof(bool)]))
        );
        // 信仰の文字列を追加する条件でModの設定を参照するように変更する
        matcher.Advance(1);
        var posFaith1 = matcher.Pos;
        var labelFaith1 = matcher.Operand;
        matcher.Advance(1);
        matcher.CreateLabel(out var labelModFaith1);
        matcher.InsertAndAdvance(
            CodeInstruction.Call(() => DisplaysFaith()),
            new CodeInstruction(OpCodes.Brtrue, labelModFaith1),
            CodeInstruction.Call(() => DisplaysAlwaysFaith()),
            new CodeInstruction(OpCodes.Brfalse, labelFaith1)
        );
        matcher.CreateLabelWithOffsets(-2, out var labelModFaith2);
        matcher.Advance(posFaith1 - matcher.Pos);
        matcher.Operand = labelModFaith2;

        // call static Chara EClass::get_pc() [Label30]
        // ldc.i4 6607
        // ldc.i4.0 NULL
        // callvirt bool Card::HasElement(int ele, bool includeNagative)
        matcher.MatchEndForward(
            new CodeMatch(OpCodes.Call, AccessTools.PropertyGetter(typeof(EClass), nameof(EClass.pc))),
            new CodeMatch(OpCodes.Ldc_I4, 6607),
            new CodeMatch(OpCodes.Ldc_I4_0),
            new CodeMatch(OpCodes.Callvirt, AccessTools.Method(typeof(Card), nameof(Card.HasElement), [typeof(int), typeof(bool)]))
        );
        // 血の味の文字列を追加する条件でModの設定を参照するように変更する
        matcher.Advance(1);
        var posBloodTaste1 = matcher.Pos;
        var labelBloodTast1 = matcher.Operand;
        matcher.Advance(1);
        matcher.CreateLabel(out var labelModBloodTaste1);
        matcher.InsertAndAdvance(
            CodeInstruction.Call(() => DisplaysBloodTaste()),
            new CodeInstruction(OpCodes.Brtrue, labelModBloodTaste1),
            CodeInstruction.Call(() => DisplaysAlwaysBloodTaste()),
            new CodeInstruction(OpCodes.Brfalse, labelBloodTast1)
        );
        matcher.CreateLabelWithOffsets(-2, out var labelModBloodTaste2);
        matcher.Advance(posBloodTaste1 - matcher.Pos);
        matcher.Operand = labelModBloodTaste2;

        // call static string CraftUtil::GetBloodText(Chara c)
        // 血の味の文字列を取得する処理をModの設定を参照するものに差し替える
        matcher.MatchStartForward(
            new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(CraftUtil), nameof(CraftUtil.GetBloodText), [typeof(Chara)]))
        );
        matcher.RemoveInstruction();
        matcher.InsertAndAdvance(
            CodeInstruction.Call(() => CraftUtilPatch.CraftUtilGetBloodTextForCharaHoverText(default!))
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
        // if ((knowFav && CharaPatch.DisplayFavorite()) || CharaPatch.DisplayAlwaysFavorite() || )
        // {
        // ...
        // text = text + $"<size=14>♡" + GetFavCat().GetName().ToLower() + "/" + GetFavFood().GetName() + "</size>";
        // ...
        // if ((EClass.pc.held?.trait is TraitWhipLove && IsPCFaction && CharaPatch.DisplayHobby()) || CharaPatch.DisplayAlwaysHobby())
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
        matcher.MatchEndForward(
            new CodeMatch(OpCodes.Ldarg_0),
            new CodeMatch(OpCodes.Call, AccessTools.PropertyGetter(typeof(Chara), nameof(Chara.knowFav)))
        );
        // 好物の文字列を追加する条件でModの設定を参照するように変更する
        matcher.Advance(1);
        var pos1 = matcher.Pos;
        var label2 = matcher.Operand;
        matcher.Advance(1);
        matcher.CreateLabel(out var labelMod1);
        matcher.InsertAndAdvance(
            CodeInstruction.Call(() => DisplaysFavorite()),
            new CodeInstruction(OpCodes.Brtrue, labelMod1),
            CodeInstruction.Call(() => DisplaysAlwaysFavorite()),
            new CodeInstruction(OpCodes.Brfalse, label2)
        );
        matcher.CreateLabelWithOffsets(-2, out var labelMod2);
        matcher.Advance(pos1 - matcher.Pos);
        matcher.Operand = labelMod2;

        // ldstr "<size=14>"
        // ldstr "favgift"
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

        // isinst TraitWhipLove [Label16]
        matcher.MatchEndForward(
            new CodeMatch(OpCodes.Isinst, typeof(TraitWhipLove))
        );
        // 趣味・仕事の文字列を追加する条件でModの設定を参照するように変更する
        matcher.Advance(1);
        var pos2 = matcher.Pos;
        var label3 = matcher.Operand;
        matcher.Advance(1);
        matcher.CreateLabel(out var labelMod3);
        matcher.InsertAndAdvance(
            CodeInstruction.Call(() => DisplaysHobby()),
            new CodeInstruction(OpCodes.Brtrue, labelMod3),
            CodeInstruction.Call(() => DisplaysAlwaysHobby()),
            new CodeInstruction(OpCodes.Brfalse, label3)
        );
        matcher.CreateLabelWithOffsets(-2, out var labelMod4);
        matcher.Advance(pos2 - matcher.Pos);
        matcher.Operand = labelMod4;

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
        return !StyleConfig.DisableShadowform;
    }

    private static bool IsMimicryEnabled()
    {
        return !StyleConfig.DisableMimicry;
    }

    private static bool DisplaysLvComparison()
    {
        return StyleConfig.DisplayLvComparison;
    }

    private static bool DisplaysFactionMemberType()
    {
        return StyleConfig.DisplayFactionMemberType;
    }

    private static bool DisplaysHeightDifference()
    {
        return StyleConfig.DisplayHeightDifference;
    }

    private static bool DisplaysMilkBaby()
    {
        return StyleConfig.DisplayMilkBaby;
    }

    private static bool DisplaysBounty()
    {
        return StyleConfig.DisplayBounty;
    }

    private static bool DisplaysFaith()
    {
        return StyleConfig.DisplayFaith == ModItemDisplayMode.Show;
    }

    private static bool DisplaysAlwaysFaith()
    {
        return StyleConfig.DisplayFaith == ModItemDisplayMode.AlwaysShow;
    }

    private static bool DisplaysBloodTaste()
    {
        return StyleConfig.DisplayBloodTaste == ModItemDisplayMode.Show;
    }

    private static bool DisplaysAlwaysBloodTaste()
    {
        return StyleConfig.DisplayBloodTaste == ModItemDisplayMode.AlwaysShow;
    }

    private static bool DisplaysFavorite()
    {
        return StyleConfig.DisplayFavorite == ModItemDisplayMode.Show;
    }

    private static bool DisplaysAlwaysFavorite()
    {
        return StyleConfig.DisplayFavorite == ModItemDisplayMode.AlwaysShow;
    }

    private static bool DisplaysHobby()
    {
        return StyleConfig.DisplayHobby == ModItemDisplayMode.Show;
    }

    private static bool DisplaysAlwaysHobby()
    {
        return StyleConfig.DisplayHobby == ModItemDisplayMode.AlwaysShow;
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
            text3 = text3[..^Environment.NewLine.Length];
            textEnd = "</size>";
        }
        return $"{text3[..text3.LastIndexOf(textEnd)]}</size>";
    }

    private static string BuildHoverText(string text, string text2, string s, Chara chara)
    {
        text = text.TagResize(ComputeFontSize);
        text2 = text2.TrimTagTexts().TagResize(ComputeFontSize);
        s = s.TrimTagTexts().TagResize(ComputeFontSize);
        return ModCharaHoverTextBuilder.BuildHoverText(chara, text, text2, s);
    }

    private static string BuildHoverText2(string text, string text2, string text3, Chara chara)
    {
        text = text.TagResize(ComputeFontSize);
        text2 = text2.TagResize(ComputeFontSize);
        text3 = text3.TagResize(ComputeFontSize);
        return ModCharaHoverTextBuilder.BuildHoverText2(chara, text, text2, text3);
    }

    [HarmonyPatch(typeof(CraftUtil))]
    public static class CraftUtilPatch
    {
        private static readonly ModPatchTarget _patchTarget = new();

        [HarmonyPrepare]
        private static bool Prepare(MethodBase? original)
        {
            return _patchTarget.IsPatchable(original) && ModContext.Config.EnableHoverGuide.Value;
        }

        [HarmonyReversePatch(HarmonyReversePatchType.Original)]
        [HarmonyPatch(nameof(CraftUtil.GetBloodText), [typeof(Chara)])]
        public static string CraftUtilGetBloodTextForCharaHoverText(Chara c)
        {
            // CraftUtil.GetBloodText()のコードを複製し、ホバーテキスト取得処理向けに変更したスタブを作成する
            static IEnumerable<CodeInstruction> transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
            {
                // // 変更前
                // int num = Mathf.Min(list.Count(), 3, EClass.debug.godMode ? 3 : (1 + EClass.pc.Evalue(6607) / 15));
                // // 変更後
                // int num = Mathf.Min(list.Count(), 3, EClass.debug.godMode || CharaPatch.DisplaysAlwaysBloodTaste() ? 3 : (1 + EClass.pc.Evalue(6607) / 15));
                var matcher = new CodeMatcher(instructions, generator);
                
                // ldfld bool CoreDebug::godMode
                matcher.MatchStartForward(
                    new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(CoreDebug), nameof(CoreDebug.godMode)))
                );
                matcher.Advance(1);
                var label1 = matcher.Operand;
                matcher.Advance(1);
                matcher.InsertAndAdvance(
                    CodeInstruction.Call(() => DisplaysAlwaysBloodTaste()),
                    new CodeInstruction(OpCodes.Brtrue, label1)
                );

                return matcher.InstructionEnumeration();
            }

            _ = transpiler(null!, null!);
            return default!;
        }
    }
}
