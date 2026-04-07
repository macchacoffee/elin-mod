using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Empyrean.Utils;
using HarmonyLib;
using ModUtility.Patch;
using UnityEngine;

namespace SomewhatEnhancedDisplay.Patches;

[HarmonyPatch(typeof(Chara))]
public static class CharaPatch
{
    private static readonly Color HPColor = new(0.872f, 0.371f, 0.335f);
    // private static readonly Color32 HPColor = new(222, 95, 85, 255);
    private static readonly Color HPLightenColor = new(0.982f, 0.701f, 0.665f);
    // private static readonly Color32 HPLightenColor = new(250, 179, 170, 255);
    private static readonly Color ManaColor = new(0.375f, 0.606f, 0.988f);
    // private static readonly Color32 ManaColor = new(96, 155, 252, 255);
    private static readonly Color ManaLightenColor = new(0.665f, 0.806f, 0.838f);
    // private static readonly Color32 ManaLightenColor = new(170, 206, 214, 255);
    private static readonly Color StaminaColor = new(0.848f, 0.722f, 0.285f);
    // private static readonly Color32 StaminaColor = new(216, 184, 73, 255);
    private static readonly Color StaminaLightenColor = new(0.848f, 0.82f, 0.635f);
    // private static readonly Color32 StaminaLightenColor = new(216, 209, 162, 255);
    private static readonly Color ResistColor = new(0.375f, 0.738f, 0.626f);
    // private static readonly Color32 ResistColor = new(96, 188, 160, 255);
    private static readonly Color NegativeResistColor = new(0.822f, 0.431f, 0.395f);
    // private static readonly Color32 NegativeResistColor = new(210, 110, 101, 255);
    private static readonly Color NoneResistColor = new(0.7f, 0.7f, 0.7f);
    // private static readonly Color32 NoneResistColor = new(178, 178, 178, 255);
    private static readonly Color SubColor1 = new(0.7f, 0.7f, 0.7f);
    // private static readonly Color32 SubColor1 = new(178, 178, 178, 255);

    private static readonly int LowValueThreshold = 10;

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
        // // 変更後
        // return CharaPatch.BuildHoverText(text, text2, s, this);
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
            CodeInstruction.Call(() => BuildHoverText(default!, default!, default!, default!))
        );

        return matcher.InstructionEnumeration();
    }

    [HarmonyTranspiler]
    [HarmonyPatch(nameof(Chara.GetHoverText2), [])]
    private static IEnumerable<CodeInstruction> GetHoverText2_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        // // 変更前
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
        // text = text + "<size=14>♥" + GetFavCat().GetName().ToLower() + "/" + GetFavFood().GetName() + "</size>";
        // ...
        // text4 = CharaPatch.GetHoverText2StatsExtra(text4, item3);
        // text3 = text3 + text4.TagColor(c) + ", ";
        // ...
        // else
        // {
        //      text3 = text3.TrimEnd(", ".ToCharArray()) + "</size>";
        // }
        // ...
        // return CharaPatch.BuildHoverText(text, text2, text3, this);
        var matcher = new CodeMatcher(instructions, generator);

        matcher.MatchStartForward(
            new CodeMatch(OpCodes.Ldstr),
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
            CodeInstruction.Call(() => GetHoverText2StatsExtra(default!, default!)),
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

    private static string BuildHoverText(string text, string text2, string s, Chara chara)
    {
        // text1: 名前
        // text2: レベル差、赤ちゃん、高低差、賞金首、信仰
        // s: ゲスト・家畜、血の風味
        text = text.TagSize(18);
        text2 = text2.IsEmpty() ? text2 : text2.TagColor(SubColor1);
        s = s.IsEmpty() ? s : s.TagColor(SubColor1);
        return $"{GetHoverTextLv(chara)} {text}{text2}{s}";

    }

    private static string GetHoverText2StatsExtra(string text4, BaseStats stats)
    {
        var statsValueText = $"({stats.GetValue()})".TagSize(12);
        return $"{text4}{statsValueText}";
    }

    private static string BuildHoverText2(string text, string text2, string text3, Chara chara)
    {
        // text: 好物
        // text2: 趣味・仕事
        // text3: バフ・デバフ・状態・呪い
        text = text.StartsWith(Environment.NewLine) ? text.Substring(Environment.NewLine.Length) : text;
        text2 = text2.StartsWith(Environment.NewLine) ? text2.Substring(Environment.NewLine.Length) : text2;
        text3 = text3.StartsWith(Environment.NewLine) ? text3.Substring(Environment.NewLine.Length) : text3;
        return string.Join(Environment.NewLine, new[] {
            // GetHoverTextProfile1(chara, text2).TagColor(SubColor1),
            // GetHoverTextProfile2(chara, text).TagColor(SubColor1),
            GetHoverTextAttr1(chara),
            GetHoverTextAttr2(chara),
            // GetHoverTextAttr3(chara).TagColor(SubColor1),
            // GetHoverTextFeat(chara)?.TagColor(SubColor1),
            GetHoverTextAct(chara),
            GetHoverTextResist(chara),
            text3,
        }.Where(l => !string.IsNullOrEmpty(l)));
    }

    private static string GetHoverTextLv(Chara chara)
    {
        return GetLvText(chara);
    }

    private static string GetHoverTextProfile1(Chara chara, string hobby)
    {
        return string.Join(" ", new[] {
            GetGenderText(chara),
            GetAgeText(chara),
            GetRaceText(chara),
            $"{GetJobText(chara)}/{GetTacticsText(chara)}",
            hobby,
        }.Where(l => !string.IsNullOrEmpty(l))).TagSize(14);
    }

    private static string GetHoverTextProfile2(Chara chara, string fav)
    {
        return string.Join(" ", new[] {
            GetAffinityText(chara),
            fav,
        }.Where(l => !string.IsNullOrEmpty(l))).TagSize(14);
    }

    private static string GetHoverTextAttr1(Chara chara)
    {
        return string.Join(" ", new[] {
            GetHPText(chara),
            GetManaText(chara),
            GetStaminaText(chara),
        }.Where(l => !string.IsNullOrEmpty(l))).TagSize(14);
    }

    private static string GetHoverTextAttr2(Chara chara)
    {
        return string.Join(" ", new[] {
            GetDVText(chara),
            GetPVText(chara),
            GetSkillSpdText(chara),
            GetExpText(chara),
            GetMainElementText(chara),
        }.Where(l => !string.IsNullOrEmpty(l))).TagSize(14);
    }

    private static string GetHoverTextAttr3(Chara chara)
    {
        return string.Join(" ", new[] {
            GetSkillStrText(chara),
            GetSkillEndText(chara),
            GetSkillDexText(chara),
            GetSkillPerText(chara),
            GetSkillLerText(chara),
            GetSkillWilText(chara),
            GetSkillMagText(chara),
            GetSkillChaText(chara),
        }.Where(l => !string.IsNullOrEmpty(l))).TagSize(14);
    }

    private static string? GetHoverTextFeat(Chara chara)
    {
        return GetFeatListText(chara);
    }

    private static string? GetHoverTextAct(Chara chara)
    {
        return GetActListText(chara);
    }

    private static string? GetHoverTextResist(Chara chara)
    {
        return GetResistListText(chara);
    }

    private static string GetLvText(Chara chara)
    {
        return $"Lv.{chara.LV}".TagSize(14);
    }

    private static string GetGenderText(Chara chara)
    {
        return Lang._gender(chara.bio.gender).TagSize(14);
    }

    private static string GetAgeText(Chara chara)
    {
        return chara.bio.TextAge(chara).TagSize(14);
    }

    private static string GetRaceText(Chara chara)
    {
        return chara.race.GetName().TagSize(14);
    }

    private static string GetJobText(Chara chara)
    {
        return chara.job.GetName().TagSize(14);
    }

    private static string GetTacticsText(Chara chara)
    {
        return chara.tactics.source.GetName().TagSize(14);
    }

    private static string GetAffinityText(Chara chara)
    {
        return $"{chara.affinity.Name}{$"({chara._affinity})".TagSize(12)}";
    }

    private static string GetHPText(Chara chara)
    {
        var hpValueColor = Math.Ceiling((float)chara.hp / chara.MaxHP * 100) > LowValueThreshold ? HPLightenColor : HPLightenColor.Darken(0.2f);
        var hpText = "HP:".TagColor(HPColor).TagSize(14);
        var hpValueText = $"{chara.hp}/{chara.MaxHP}".TagColor(hpValueColor).TagSize(16);
        return $"{hpText}{hpValueText}";
    }

    private static string GetManaText(Chara chara)
    {
        var manaValueColor = Math.Ceiling((float)chara.mana.value / chara.mana.max * 100) > LowValueThreshold ? ManaLightenColor : ManaLightenColor.Darken(0.2f);
        var manaText = "MP:".TagColor(ManaColor).TagSize(14);
        var manaValueText = $"{chara.mana.value}/{chara.mana.max}".TagColor(manaValueColor).TagSize(16);
        return $"{manaText}{manaValueText}";
    }

    private static string GetStaminaText(Chara chara)
    {
        var staminaValueColor = Math.Ceiling((float)chara.stamina.value / chara.stamina.max * 100) > LowValueThreshold ? StaminaLightenColor : StaminaLightenColor.Darken(0.2f);
        var staminaText = "ST:".TagColor(StaminaColor).TagSize(14);
        var staminaValuetext = $"{chara.stamina.value}/{chara.stamina.max}".TagColor(staminaValueColor).TagSize(16);
        return $"{staminaText}{staminaValuetext}";
    }

    private static string GetDVText(Chara chara)
    {
        var dvText = "DV:".TagSize(14);
        var dvValueTest = $"{chara.DV}".TagSize(16);
        return $"{dvText}{dvValueTest}";
    }

    private static string GetPVText(Chara chara)
    {
        var pvText = "PV:".TagSize(14);
        var pvValueTest = $"{chara.PV}".TagSize(16);
        return $"{pvText}{pvValueTest}";
    }

    private static string GetSkillSpdText(Chara chara)
    {
        var spd = chara.elements.GetElement(SKILL.SPD);
        var spdText = $"{spd.Name}:".TagSize(14);
        var spdValueTest = $"{spd.Value}".TagSize(16);
        return $"{spdText}{spdValueTest}";
    }

    private static string GetExpText(Chara chara)
    {
        var expText = "EXP:".TagSize(14);
        var expValueText =  $"{chara.exp}/{chara.ExpToNext}".TagSize(16);
        return $"{expText}{expValueText}";
    }

    private static string? GetMainElementText(Chara chara)
    {
        var mainElement = chara.MainElement;
        if (mainElement == Element.Void)
        {
            return null;
        }
        var mainElementText = $"[{mainElement.Name}]";
        if (EClass.Colors.elementColors.TryGetValue(mainElement.source.alias, out var color))
        {
            mainElementText = mainElementText.TagColor(Color.Lerp(color, Color.white, 0.4f));
        }
        return mainElementText.TagSize(14);
    }

    private static string GetSkillStrText(Chara chara)
    {
        return GetSkillAttributeText(chara, SKILL.STR);
    }

    private static string GetSkillEndText(Chara chara)
    {
        return GetSkillAttributeText(chara, SKILL.END);
    }

    private static string GetSkillDexText(Chara chara)
    {
        return GetSkillAttributeText(chara, SKILL.DEX);
    }

    private static string GetSkillPerText(Chara chara)
    {
        return GetSkillAttributeText(chara, SKILL.PER);
    }

    private static string GetSkillLerText(Chara chara)
    {
        return GetSkillAttributeText(chara, SKILL.LER);
    }

    private static string GetSkillWilText(Chara chara)
    {
        return GetSkillAttributeText(chara, SKILL.WIL);
    }

    private static string GetSkillMagText(Chara chara)
    {
        return GetSkillAttributeText(chara, SKILL.MAG);
    }

    private static string GetSkillChaText(Chara chara)
    {
        return GetSkillAttributeText(chara, SKILL.CHA);
    }

    private static string GetSkillAttributeText(Chara chara, int attribute)
    {
        var attr = chara.elements.GetElement(attribute);
        return $"{attr.Name}:{attr.Value}".TagSize(14);
    }

    private static string? GetFeatListText(Chara chara)
    {
        var feats = chara.elements.ListElements(e => e.source.category == "feat" && e.Value > 0);
        if (!feats.Any())
        {
            return null;
        }

        return string.Join(
            ", ",
            feats.Select(f => $"{f.Name}{(f.Value > 1 ? $"({f.Value})".TagSize(12) : "")}")
        ).TagSize(14);
    }

    private static string? GetActListText(Chara chara)
    {
        if (!chara.ability.list.items.Any())
        {
            return null;
        }

        return string.Join(
            ", ",
            chara.ability.list.items.Select(a => $"{a.act.Name}{(a.pt ? "(pt)".TagSize(12) : "")}")
        ).TagSize(14);
    }

    private static string? GetResistListText(Chara chara)
    {
        var resists = chara.elements.ListElements(e => e.source.category == "resist" && e.Value != 0);
        if (!resists.Any())
        {
            return null;
        }

        return string.Join(
            Environment.NewLine,
            resists
                .GroupBy(r => Element.GetResistLv(r.Value))
                .Where(g => g.Key != (int)Resist.None)
                .OrderByDescending(g => g.Key)
                .Select(g => $"{GetResistLevelText(g.Key)}: {
                    string.Join(", ", g.OrderBy(r => r.id).Select(GetResistText).Where(t => !string.IsNullOrEmpty(t)))
                }".TagSize(14).TagColor(GetResistColor(g.Key)))
        );
    }

    private static Color GetResistColor(int resistLevel)
    {
        if (resistLevel > (int)Resist.None)
        {
            return Color.Lerp(NoneResistColor, ResistColor, 1 * (resistLevel / (float)Resist.Immune));
        }
        else if (resistLevel < (int)Resist.None)
        {
            return Color.Lerp(NoneResistColor, NegativeResistColor, 1 * (resistLevel / (float)Resist.CriticalWeakness));
        }
        else
        {
            return NoneResistColor;
        }
    }

    private static string GetResistLevelText(int resistLevel)
    {
        var level = (Resist)Math.Max(Mathf.Min(resistLevel, (int)Resist.Immune), (int)Resist.CriticalWeakness);
        return level switch
        {
            Resist.Immune => "+20",
            Resist.Great => "+15",
            Resist.Strong => "+10",
            Resist.Normal => "+5",
            Resist.None => "0",
            Resist.Weakness => "-5",
            Resist.CriticalWeakness => "-10",
            _ => throw new ArgumentException(),
        };
        // return resistLevel <= 0 ? Lang.GetList("resistNeg")[-resistLevel] : Lang.GetList("resist")[Mathf.Min(resistLevel, (int)Resist.Immune + 1)];
    }

    private static string? GetResistText(Element resist)
    {
        var eleAlias = resist.source.aliasParent;
        if (eleAlias is null || !eleAlias.StartsWith("ele"))
        {
            return null;
        }
        // エーテル耐性が25以上の場合はエーテル病が進行しないことを示すため、"*"を追加する
        var resImmunePlusText = resist.id == SKILL.resEther && resist.Value >= 25 ? "*" : string.Empty;
        if (!EClass.sources.elements.alias.TryGetValue(eleAlias, out var element))
        {
            return null;
        }

        return $"{resImmunePlusText}{element.GetName()}{resImmunePlusText}";
    }
}
