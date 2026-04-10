using System;
using System.Linq;
using Empyrean.Utils;
using UnityEngine;

namespace SomewhatEnhancedDisplay.UI;

public static class ModHoverTextBuilder
{
    private static readonly Color HPColor = new(0.872f, 0.371f, 0.335f);
    private static readonly Color HPLightenColor = new(0.982f, 0.701f, 0.665f);
    private static readonly Color ManaColor = new(0.375f, 0.606f, 0.988f);
    private static readonly Color ManaLightenColor = new(0.665f, 0.806f, 0.838f);
    private static readonly Color StaminaColor = new(0.848f, 0.722f, 0.285f);
    private static readonly Color StaminaLightenColor = new(0.848f, 0.82f, 0.635f);
    private static readonly Color ResistColor = new(0.375f, 0.738f, 0.626f);
    private static readonly Color NegativeResistColor = new(0.822f, 0.431f, 0.395f);
    private static readonly Color NoneResistColor = new(0.7f, 0.7f, 0.7f);
    private static readonly Color SubColor1 = new(0.7f, 0.7f, 0.7f);

    private static readonly int LowValueThreshold = 10;

    public static string BuildHoverText(string text, string text2, string s, Chara chara)
    {
        // text1: 名前
        // text2: レベル差、赤ちゃん、高低差、賞金首、信仰
        // s: ゲスト・家畜、血の風味
        text2 = text2.IsEmpty() ? text2 : text2.TagColor(SubColor1);
        s = s.IsEmpty() ? s : s.TagColor(SubColor1);
        return string.Join(" ", new[] {
            GetHoverTextLv(chara)?.TagSize(ModUIUtil.ComputeFontSize(11)),
            $"{text}{text2}{s}",
        }.Where(t => !string.IsNullOrEmpty(t)));
    }

    public static string BuildHoverText2(string text, string text2, string text3, Chara chara)
    {
        // text: 好物
        // text2: 趣味・仕事
        // text3: バフ・デバフ・状態・呪い
        text = text.StartsWith(Environment.NewLine) ? text.Substring(Environment.NewLine.Length) : text;
        text2 = text2.StartsWith(Environment.NewLine) ? text2.Substring(Environment.NewLine.Length) : text2;
        text3 = text3.StartsWith(Environment.NewLine) ? text3.Substring(Environment.NewLine.Length) : text3;
        return string.Join(Environment.NewLine, new[] {
            GetHoverTextProfile1(chara, text2)?.TagSize(ModUIUtil.ComputeFontSize(11)).TagColor(SubColor1),
            GetHoverTextProfile2(chara, text)?.TagSize(ModUIUtil.ComputeFontSize(11)).TagColor(SubColor1),
            GetHoverStatusAttribute(chara)?.TagSize(ModUIUtil.ComputeFontSize(13)),
            GetHoverStatus(chara)?.TagSize(ModUIUtil.ComputeFontSize(13)),
            GetHoverTextPrimaryAttribute(chara)?.TagSize(ModUIUtil.ComputeFontSize(11)).TagColor(SubColor1),
            GetHoverTextFeat(chara)?.TagSize(ModUIUtil.ComputeFontSize(11)).TagColor(SubColor1),
            GetHoverTextAct(chara)?.TagSize(ModUIUtil.ComputeFontSize(11)),
            GetHoverTextResist(chara)?.TagSize(ModUIUtil.ComputeFontSize(11)),
            text3,
        }.Where(t => !string.IsNullOrEmpty(t)));
    }

    public static string BuildStatsExtraText(string text4, BaseStats stats)
    {
        var statsValueText = $"({stats.GetValue()})".TagSize(ModUIUtil.ComputeFontSize(9));
        return $"{text4}{statsValueText}";
    }

    public static string BuildOtherCardsText(string hoverText, string otherCardsText)
    {
        return $"{hoverText}{otherCardsText.TagSize(ModUIUtil.ComputeFontSize(11))}";
    }

    private static string? GetHoverTextLv(Chara chara)
    {
        return Mod.Config.HoverGuide.CurrentProfile.DisplayLv ? GetLvText(chara) : null;
    }

    private static string? GetHoverTextProfile1(Chara chara, string hobby)
    {
        var text = string.Join(" ", new[] {
            Mod.Config.HoverGuide.CurrentProfile.DisplayGender ? GetGenderText(chara) : null,
            Mod.Config.HoverGuide.CurrentProfile.DisplayAge ? GetAgeText(chara) : null,
            Mod.Config.HoverGuide.CurrentProfile.DisplayRace ? GetRaceText(chara) : null,
            Mod.Config.HoverGuide.CurrentProfile.DisplayJobTactics ? $"{GetJobText(chara)}/{GetTacticsText(chara)}" : null,
            Mod.Config.HoverGuide.CurrentProfile.DisplayHobby ? hobby : null,
        }.Where(t => !string.IsNullOrEmpty(t)));
        return !string.IsNullOrEmpty(text) ? text : null;
    }

    private static string? GetHoverTextProfile2(Chara chara, string fav)
    {
        var text = string.Join(" ", new[] {
            Mod.Config.HoverGuide.CurrentProfile.DisplayGender ? GetAffinityText(chara) : null,
            Mod.Config.HoverGuide.CurrentProfile.DisplayFavorite ? fav: null,
        }.Where(t => !string.IsNullOrEmpty(t)));
        return !string.IsNullOrEmpty(text) ? text : null;
    }

    private static string? GetHoverStatusAttribute(Chara chara)
    {
        var text = string.Join(" ", new[] {
            Mod.Config.HoverGuide.CurrentProfile.DisplayHP ? GetHPText(chara) : null,
            Mod.Config.HoverGuide.CurrentProfile.DisplayMana ? GetManaText(chara) : null,
            Mod.Config.HoverGuide.CurrentProfile.DisplayStamina ? GetStaminaText(chara) : null,
        }.Where(t => !string.IsNullOrEmpty(t)));
        return !string.IsNullOrEmpty(text) ? text : null;
    }

    private static string? GetHoverStatus(Chara chara)
    {
        var text = string.Join(" ", new[] {
            Mod.Config.HoverGuide.CurrentProfile.DisplayDVPV ? GetDVText(chara) : null,
            Mod.Config.HoverGuide.CurrentProfile.DisplayDVPV ? GetPVText(chara) : null,
            Mod.Config.HoverGuide.CurrentProfile.DisplaySpeed ? GetSkillSpdText(chara) : null,
            Mod.Config.HoverGuide.CurrentProfile.DisplayExp ? GetExpText(chara) : null,
            Mod.Config.HoverGuide.CurrentProfile.DisplayMainElement ? GetMainElementText(chara) : null,
        }.Where(t => !string.IsNullOrEmpty(t)));
        return !string.IsNullOrEmpty(text) ? text : null;
    }

    private static string? GetHoverTextPrimaryAttribute(Chara chara)
    {
        if (!Mod.Config.HoverGuide.CurrentProfile.DisplayPrimaryAttributes)
        {
            return null;
        }
        var text = string.Join(" ", new[] {
            GetSkillStrText(chara),
            GetSkillEndText(chara),
            GetSkillDexText(chara),
            GetSkillPerText(chara),
            GetSkillLerText(chara),
            GetSkillWilText(chara),
            GetSkillMagText(chara),
            GetSkillChaText(chara),
        }.Where(t => !string.IsNullOrEmpty(t)));
        return !string.IsNullOrEmpty(text) ? text : null;
    }

    private static string? GetHoverTextFeat(Chara chara)
    {
        return Mod.Config.HoverGuide.CurrentProfile.DisplayFeat ? GetFeatListText(chara) : null;
    }

    private static string? GetHoverTextAct(Chara chara)
    {
        return Mod.Config.HoverGuide.CurrentProfile.DisplayAct ? GetActListText(chara) : null;
    }

    private static string? GetHoverTextResist(Chara chara)
    {
        return Mod.Config.HoverGuide.CurrentProfile.DisplayResist ? GetResistListText(chara) : null;
    }

    private static string GetLvText(Chara chara)
    {
        return $"Lv.{chara.LV}";
    }

    private static string GetGenderText(Chara chara)
    {
        return Lang._gender(chara.bio.gender);
    }

    private static string GetAgeText(Chara chara)
    {
        return chara.bio.TextAge(chara);
    }

    private static string GetRaceText(Chara chara)
    {
        return chara.race.GetName();
    }

    private static string GetJobText(Chara chara)
    {
        return chara.job.GetName();
    }

    private static string GetTacticsText(Chara chara)
    {
        return chara.tactics.source.GetName();
    }

    private static string GetAffinityText(Chara chara)
    {
        return $"{chara.affinity.Name}{$"({chara._affinity})".TagSize(ModUIUtil.ComputeFontSize(9))}";
    }

    private static string GetHPText(Chara chara)
    {
        var hpValueColor = Math.Ceiling((float)chara.hp / chara.MaxHP * 100) > LowValueThreshold ? HPLightenColor : HPLightenColor.Darken(0.2f);
        var hpText = "HP:".TagColor(HPColor).TagSize(ModUIUtil.ComputeFontSize(11));
        var hpValueText = $"{chara.hp}/{chara.MaxHP}".TagColor(hpValueColor);
        return $"{hpText}{hpValueText}";
    }

    private static string GetManaText(Chara chara)
    {
        var manaValueColor = Math.Ceiling((float)chara.mana.value / chara.mana.max * 100) > LowValueThreshold ? ManaLightenColor : ManaLightenColor.Darken(0.2f);
        var manaText = "MP:".TagColor(ManaColor).TagSize(ModUIUtil.ComputeFontSize(11));
        var manaValueText = $"{chara.mana.value}/{chara.mana.max}".TagColor(manaValueColor);
        return $"{manaText}{manaValueText}";
    }

    private static string GetStaminaText(Chara chara)
    {
        var staminaValueColor = Math.Ceiling((float)chara.stamina.value / chara.stamina.max * 100) > LowValueThreshold ? StaminaLightenColor : StaminaLightenColor.Darken(0.2f);
        var staminaText = "SP:".TagColor(StaminaColor).TagSize(ModUIUtil.ComputeFontSize(11));
        var staminaValuetext = $"{chara.stamina.value}/{chara.stamina.max}".TagColor(staminaValueColor);
        return $"{staminaText}{staminaValuetext}";
    }

    private static string GetDVText(Chara chara)
    {
        var dvText = "DV:".TagSize(ModUIUtil.ComputeFontSize(11));
        var dvValueTest = $"{chara.DV}";
        return $"{dvText}{dvValueTest}";
    }

    private static string GetPVText(Chara chara)
    {
        var pvText = "PV:".TagSize(ModUIUtil.ComputeFontSize(11));
        var pvValueTest = $"{chara.PV}";
        return $"{pvText}{pvValueTest}";
    }

    private static string GetSkillSpdText(Chara chara)
    {
        var spd = chara.elements.GetElement(SKILL.SPD);
        var spdText = $"{spd.Name}:".TagSize(ModUIUtil.ComputeFontSize(11));
        var spdValueTest = $"{spd.Value}";
        return $"{spdText}{spdValueTest}";
    }

    private static string GetExpText(Chara chara)
    {
        var expText = "EXP:".TagSize(ModUIUtil.ComputeFontSize(11));
        var expValueText = $"{chara.exp}/{chara.ExpToNext}";
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
        if (GetElementColor(mainElement.source.alias) is Color color)
        {
            mainElementText = mainElementText.TagColor(color);
        }
        return mainElementText.TagSize(ModUIUtil.ComputeFontSize(11));
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
        return $"{attr.Name}:{attr.Value}";
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
            feats.Select(f => $"{f.Name}{(f.Value > 1 ? $"({f.Value})".TagSize(ModUIUtil.ComputeFontSize(9)) : "")}")
        );
    }

    private static string? GetActListText(Chara chara)
    {
        if (!chara.ability.list.items.Any())
        {
            return null;
        }

        return string.Join(
            ", ",
            chara.ability.list.items.Select(a => $"{a.act.Name}{(a.pt ? "(pt)".TagSize(ModUIUtil.ComputeFontSize(9)) : "")}")
        );
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
                .Select(g => $"{$"{GetResistLevelText(g.Key)}:".TagColor(GetResistColor(g.Key))} {string.Join(", ", g.OrderBy(r => r.id).Select(GetResistText).Where(t => !string.IsNullOrEmpty(t)))}")
        );
    }

    private static Color? GetElementColor(string alias)
    {
        if (EClass.Colors.elementColors.TryGetValue(alias, out var color))
        {
            return Color.Lerp(color, Color.white, 0.4f);
        }
        return null;
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

        // var resistText = $"{resImmunePlusText}{element.GetName()}{resImmunePlusText}{$"({resist.Value})".TagSize(ModUIUtil.ComputeFontSize(9))}";
        var resistText = $"{resImmunePlusText}{element.GetName()}{resImmunePlusText}";
        if (GetElementColor(eleAlias) is Color color)
        {
            resistText = resistText.TagColor(color);
        }
        return resistText;
    }
}
