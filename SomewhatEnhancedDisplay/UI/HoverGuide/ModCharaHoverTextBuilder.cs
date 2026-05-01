using System;
using System.Collections.Generic;
using System.Linq;
using Empyrean.Utils;
using UnityEngine;
using ModUtility.Extensions;
using SomewhatEnhancedDisplay.Extensions;
using SomewhatEnhancedDisplay.Config;

namespace SomewhatEnhancedDisplay.UI.HoverGuide;

public static class ModCharaHoverTextBuilder
{
    private static readonly int LowValueThreshold = 10;

    private static ModConfigHoverGuide Config => Mod.Config.HoverGuide;
    private static ModConfigHoverGuideColorSet ColorConfig => Config.ColorSet;
    private static ModConfigHoverGuideStyleChara StyleConfig => Config.CurrentStyle.Chara;

    public static string BuildHoverText(Chara chara, string text, string text2, string s)
    {
        // text: 名前
        // text2: レベル差、赤ちゃん、高低差、賞金首、信仰
        // s: ゲスト・家畜、血の風味
        chara = StyleConfig.EnableMimicry ? chara.MimicryOrSelf : chara;
        var hoverText = string.Join(" ", new[] {
            GetHoverTextType(chara),
            GetHoverTextLv(chara),
            GetHoverTextName(text),
        }.Where(t => !string.IsNullOrEmpty(t)));
        return ModCardHoverTextBuilder.BuildHoverTextSection(hoverText, $"{text2}{s}");
    }

    public static string BuildHoverText2(Chara chara, string text, string text2, string text3)
    {
        // text: 好物
        // text2: 趣味・仕事
        // text3: バフ・デバフ・状態・呪い
        var realChara = chara;
        chara = StyleConfig.EnableMimicry ? chara.MimicryOrSelf : chara;
        text = text.StartsWith(Environment.NewLine) ? text.Substring(Environment.NewLine.Length) : text;
        text2 = text2.StartsWith(Environment.NewLine) ? text2.Substring(Environment.NewLine.Length) : text2;
        text3 = text3.StartsWith(Environment.NewLine) ? text3.Substring(Environment.NewLine.Length) : text3;
        return ModCardHoverTextBuilder.BuildHoverText(
            ModCardHoverTextBuilder.BuildHoverTextSection(
                GetHoverTextProfile1(chara, text2),
                GetHoverTextProfile2(chara, text)
            ),
            ModCardHoverTextBuilder.BuildHoverTextSection(
                GetHoverStatusAttribute(chara, realChara),
                GetHoverStatus(chara)
            ),
            ModCardHoverTextBuilder.BuildHoverTextSection(
                GetHoverTextPrimaryAttribute(chara),
                GetHoverTextFeat(chara)
            ),
            GetHoverTextAct(chara),
            GetHoverTextResist(chara),
            GetHoverTextStats(text3)
        );
    }

    private static string? GetHoverTextType(Chara chara)
    {
        if (!StyleConfig.DisplayType)
        {
            return null;
        }

        var bossText = chara.IsBoss ? "[BOSS]" : null;
        var rankText = chara.IsUnique ? "★" : (chara.IsElite ? "☆" : null);
        var text = $"{bossText}{rankText}";
        return !string.IsNullOrEmpty(text) ? text.TagSize(ModUIUtil.ComputeFontSize(13)) : null;
    }

    private static string? GetHoverTextLv(Chara chara)
    {
        if (!StyleConfig.DisplayLv)
        {
            return null;
        }

        var text = $"Lv.{chara.LV}";
        return !string.IsNullOrEmpty(text) ? text.TagSize(ModUIUtil.ComputeFontSize(13)) : null;
    }

    private static string? GetHoverTextName(string name)
    {
        return !string.IsNullOrEmpty(name) ? name.TagSize(ModUIUtil.ComputeFontSize(18)) : null;
    }

    private static string? GetHoverTextProfile1(Chara chara, string hobby)
    {
        var text = string.Join(" ", new[] {
            StyleConfig.DisplayGender ? GetGenderText(chara) : null,
            StyleConfig.DisplayAge ? GetAgeText(chara) : null,
            StyleConfig.DisplayRace ? GetRaceText(chara) : null,
            StyleConfig.DisplayJobTactics ? $"{GetJobText(chara)}/{GetTacticsText(chara)}" : null,
            StyleConfig.DisplayHobby ? hobby : null,
        }.Where(t => !string.IsNullOrEmpty(t)));
        return !string.IsNullOrEmpty(text) ? text.TagSize(ModUIUtil.ComputeFontSize(13)).TagColor(ColorConfig.SubTextColor) : null;
    }

    private static string? GetHoverTextProfile2(Chara chara, string fav)
    {
        var text = string.Join(" ", new[] {
            StyleConfig.DisplayAffinity ? GetAffinityText(chara) : null,
            StyleConfig.DisplayFavorite ? fav: null,
        }.Where(t => !string.IsNullOrEmpty(t)));
        return !string.IsNullOrEmpty(text) ? text.TagSize(ModUIUtil.ComputeFontSize(13)).TagColor(ColorConfig.SubTextColor) : null;
    }

    private static string? GetHoverStatusAttribute(Chara chara, Chara realChara)
    {
        var text = string.Join(" ", new[] {
            StyleConfig.DisplayHP ? GetHPText(chara, realChara) : null,
            StyleConfig.DisplayMana ? GetManaText(chara, realChara) : null,
            StyleConfig.DisplayStamina ? GetStaminaText(chara, realChara) : null,
        }.Where(t => !string.IsNullOrEmpty(t)));
        return !string.IsNullOrEmpty(text) ? text.TagSize(ModUIUtil.ComputeFontSize(16)) : null;
    }

    private static string? GetHoverStatus(Chara chara)
    {
        var text = string.Join(" ", new[] {
            StyleConfig.DisplayDVPV ? GetDVText(chara) : null,
            StyleConfig.DisplayDVPV ? GetPVText(chara) : null,
            StyleConfig.DisplaySpeed ? GetSkillSpdText(chara) : null,
            StyleConfig.DisplayExp ? GetExpText(chara) : null,
            StyleConfig.DisplayMainElement ? GetMainElementText(chara) : null,
        }.Where(t => !string.IsNullOrEmpty(t)));
        return !string.IsNullOrEmpty(text) ? text.TagSize(ModUIUtil.ComputeFontSize(16)) : null;
    }

    private static string? GetHoverTextPrimaryAttribute(Chara chara)
    {
        if (!StyleConfig.DisplayPrimaryAttributes)
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
        return !string.IsNullOrEmpty(text) ? text.TagSize(ModUIUtil.ComputeFontSize(13)).TagColor(ColorConfig.SubTextColor) : null;
    }

    private static string? GetHoverTextFeat(Chara chara)
    {
        if (!StyleConfig.DisplayFeat)
        {
            return null;
        }

        var feats = chara.elements.ListElements(e => e.source.category == "feat" && e.Value > 0);
        if (!feats.Any())
        {
            return null;
        }

        var itemsPerLine = StyleConfig.FeatLineWrapping.GetItemsPerLine(feats.Count);
        var text = string.Join(
            Environment.NewLine,
            feats.Select(f => $"{f.Name}{(StyleConfig.DisplayFeatValue && f.Value > 1 ? $"({f.Value})".TagSize(ModUIUtil.ComputeFontSize(11)) : string.Empty)}")
                .Chunk(itemsPerLine)
                .Select(chuck => string.Join(", ", chuck).TagSize(ModUIUtil.ComputeFontSize(13)))
        );
        return !string.IsNullOrEmpty(text) ? text.TagColor(ColorConfig.SubTextColor) : null;
    }

    private static string? GetHoverTextAct(Chara chara)
    {
        if (!StyleConfig.DisplayAct)
        {
            return null;
        }

        if (!chara.ability.list.items.Any())
        {
            return null;
        }

        var acts = chara.ability.list.items;
        var itemsPerLine = StyleConfig.ActLineWrapping.GetItemsPerLine(acts.Count);
        var text = string.Join(
            Environment.NewLine,
            acts.Select(a => $"{a.act.Name}{(StyleConfig.DisplayActParty && a.pt ? "(pt)".TagSize(ModUIUtil.ComputeFontSize(11)) : string.Empty)}")
                .Chunk(itemsPerLine)
                .Select(chuck => string.Join(", ", chuck).TagSize(ModUIUtil.ComputeFontSize(13)))
        );
        return !string.IsNullOrEmpty(text) ? text : null;
    }

    private static string? GetHoverTextResist(Chara chara)
    {
        if (!StyleConfig.DisplayResist)
        {
            return null;
        }

        var resists = chara.elements.ListElements(e => e.source.category == "resist" && e.Value != 0);
        if (!resists.Any())
        {
            return null;
        }

        if (StyleConfig.GroupResistByLavel)
        {
            return string.Join(
                Environment.NewLine,
                resists.GroupBy(r => Element.GetResistLv(r.Value))
                    .Where(g => g.Key != (int)Resist.None || StyleConfig.DisplayNoneResistLevel)
                    .OrderByDescending(g => g.Key)
                    .Select(g => GetResistListLineByLevelText(g))
            );
        }
        else
        {
            return GetResistListLineText(resists);
        }
    }

    private static string? GetHoverTextStats(string stats)
    {
        return StyleConfig.DisplayStats ? stats : null;
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
        return $"{chara.affinity.Name}{$"({chara._affinity})".TagSize(ModUIUtil.ComputeFontSize(13))}";
    }

    private static string GetHPText(Chara chara, Chara realChara)
    {
        // 擬人状態の場合は正体のキャラのHP割合を見かけ上のキャラの現在HPに反映する
        var ratio = (float)realChara.hp / realChara.MaxHP;
        var hp = chara == realChara ? chara.hp : (int)(chara.MaxHP * ratio);
        var hpValueColor = Math.Ceiling(ratio * 100) > LowValueThreshold ? ColorConfig.HPValueColor : ColorConfig.HPValueColor.Darken(0.2f);
        var hpText = "HP:".TagColor(ColorConfig.HPLabelColor).TagSize(ModUIUtil.ComputeFontSize(13));
        var hpValueText = $"{hp}/{chara.MaxHP}".TagColor(hpValueColor);
        return $"{hpText}{hpValueText}";
    }

    private static string GetManaText(Chara chara, Chara realChara)
    {
        // 擬人状態の場合は正体のキャラのマナ割合を見かけ上のキャラの現在マナに反映する
        var ratio = (float)realChara.mana.value / realChara.mana.max;
        var mana = chara == realChara ? chara.mana.value : (int)(chara.mana.max * ratio);
        var manaValueColor = Math.Ceiling(ratio * 100) > LowValueThreshold ? ColorConfig.ManaValueColor : ColorConfig.ManaValueColor.Darken(0.2f);
        var manaText = "MP:".TagColor(ColorConfig.ManaLabelColor).TagSize(ModUIUtil.ComputeFontSize(13));
        var manaValueText = $"{mana}/{chara.mana.max}".TagColor(manaValueColor);
        return $"{manaText}{manaValueText}";
    }

    private static string GetStaminaText(Chara chara, Chara realChara)
    {
        // 擬人状態の場合は正体のキャラのスタミナ割合を見かけ上のキャラの現在スタミナに反映する
        var ratio = (float)realChara.stamina.value / realChara.stamina.max;
        var stamina = chara == realChara ? chara.stamina.value : (int)(chara.stamina.max * ratio);
        var staminaValueColor = Math.Ceiling(ratio * 100) > LowValueThreshold ? ColorConfig.StaminaValueColor : ColorConfig.StaminaValueColor.Darken(0.2f);
        var staminaText = "SP:".TagColor(ColorConfig.StaminaLabelColor).TagSize(ModUIUtil.ComputeFontSize(13));
        var staminaValuetext = $"{stamina}/{chara.stamina.max}".TagColor(staminaValueColor);
        return $"{staminaText}{staminaValuetext}";
    }

    private static string GetDVText(Chara chara)
    {
        var dvText = "DV:".TagSize(ModUIUtil.ComputeFontSize(13));
        var dvValueTest = $"{chara.DV}";
        return $"{dvText}{dvValueTest}";
    }

    private static string GetPVText(Chara chara)
    {
        var pvText = "PV:".TagSize(ModUIUtil.ComputeFontSize(13));
        var pvValueTest = $"{chara.PV}";
        return $"{pvText}{pvValueTest}";
    }

    private static string GetSkillSpdText(Chara chara)
    {
        var spd = chara.elements.GetElement(SKILL.SPD);
        var spdText = $"{spd.Name}:".TagSize(ModUIUtil.ComputeFontSize(13));
        var spdValueTest = $"{spd.Value}";
        return $"{spdText}{spdValueTest}";
    }

    private static string GetExpText(Chara chara)
    {
        var expText = "EXP:".TagSize(ModUIUtil.ComputeFontSize(13));
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
        return mainElementText.TagSize(ModUIUtil.ComputeFontSize(13));
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

    private static string? GetResistListLineText(IEnumerable<Element> resists)
    {
        var itemsPerLine = StyleConfig.ResistLineWrapping.GetItemsPerLine(resists.Count());
        var text = string.Join(
            Environment.NewLine,
            resists.OrderByDescending(r => r.Value)
                .ThenBy(r => r.id)
                .Select(GetResistText)
                .Chunk(itemsPerLine)
                .Select(chuck => string.Join(", ", chuck).TagSize(ModUIUtil.ComputeFontSize(13)))
            );
        return !string.IsNullOrEmpty(text) ? text : null;
    }

    private static string? GetResistListLineByLevelText(IGrouping<int, Element> group)
    {
        var resistLevelText = GetResistLevelText(group.Key);
        if (!string.IsNullOrEmpty(resistLevelText))
        {
            resistLevelText = $"{resistLevelText}:".TagColor(GetResistColor(group.Key));
        }

        var itemsPerLine = StyleConfig.ResistLineWrapping.GetItemsPerLine(group.Count());
        var resistListText = string.Join(Environment.NewLine,
            group.OrderBy(r => r.id)
                .Select(GetResistText)
                .Where(t => !string.IsNullOrEmpty(t))
                .Chunk(itemsPerLine)
                .Select(chuck => string.Join(", ", chuck).TagSize(ModUIUtil.ComputeFontSize(13)))
        );
        return !string.IsNullOrEmpty(resistLevelText) && !string.IsNullOrEmpty(resistListText) ? $"{resistLevelText.TagSize(ModUIUtil.ComputeFontSize(13))} {resistListText}" : null;
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
            return Color.Lerp(ColorConfig.NoneResistLabelColor, ColorConfig.ResistLabelColor, 1 * (resistLevel / (float)Resist.Immune));
        }
        else if (resistLevel < (int)Resist.None)
        {
            return Color.Lerp(ColorConfig.NoneResistLabelColor, ColorConfig.NegativeResistLabelColor, 1 * (resistLevel / (float)Resist.CriticalWeakness));
        }
        else
        {
            return ColorConfig.NoneResistLabelColor;
        }
    }

    private static string? GetResistLevelText(int resistLevel)
    {
        var level = Math.Max(Mathf.Min(resistLevel, (int)Resist.Immune + 1), (int)Resist.CriticalWeakness);
        if (StyleConfig.UseShortResistLavelLabel)
        {
            return (Resist)level switch
            {
                Resist.Immune => "+20",
                Resist.Great => "+15",
                Resist.Strong => "+10",
                Resist.Normal => "+5",
                Resist.None => "0",
                Resist.Weakness => "-5",
                Resist.CriticalWeakness => "-10",
                _ => level == (int)Resist.Immune + 1 ? "+25" : null,
            };
        }
        else
        {
            return level <= 0 ? Lang.GetList("resistNeg")[-level] : Lang.GetList("resist")[level];
        }
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

        var resistText = $"{resImmunePlusText}{element.GetName()}{resImmunePlusText}{(StyleConfig.DisplayResistValue ? $"({resist.Value})".TagSize(ModUIUtil.ComputeFontSize(11)) : string.Empty)}";
        if (GetElementColor(eleAlias) is Color color)
        {
            resistText = resistText.TagColor(color);
        }
        return resistText;
    }
}
